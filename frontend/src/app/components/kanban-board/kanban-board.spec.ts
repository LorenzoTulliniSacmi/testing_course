import { ComponentFixture, TestBed } from "@angular/core/testing";
import { KanbanBoardComponent } from "./kanban-board";
import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { By } from "@angular/platform-browser";

const mockTasks = [
  { id: '1', title: 'Todo Task', status: 'todo', priority: 'high', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' },
  { id: '2', title: 'In Progress Task', status: 'in-progress', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' },
  { id: '3', title: 'Done Task', status: 'done', priority: 'low', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' },
  { id: '4', title: 'Archived Task', status: 'done', priority: 'high', archived: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
];

describe('KanbanBoardComponent', () => {
  let component: KanbanBoardComponent;
  let fixture: ComponentFixture<KanbanBoardComponent>;
  let httpController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KanbanBoardComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(KanbanBoardComponent);
    component = fixture.componentInstance;
    httpController = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => {
    httpController.verify();
  });

  describe('Task Display', () => {
    it('renders tasks in correct columns and excludes archived tasks', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush(mockTasks);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;
      const columns = debugEl.queryAll(By.css('app-task-column'));

      expect(columns.length).toBe(3);
      expect(columns[0].nativeElement.textContent).toContain('Todo Task');
      expect(columns[1].nativeElement.textContent).toContain('In Progress Task');
      expect(columns[2].nativeElement.textContent).toContain('Done Task');
      expect(debugEl.nativeElement.textContent).not.toContain('Archived Task');
      expect(debugEl.nativeElement.textContent).toContain('Total: 3');
    });
  });

  describe('Task Form', () => {
    it('opens form for new task and creates task on submit', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([]);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;

      // Open form
      debugEl.query(By.css('.btn-add')).nativeElement.click();
      fixture.detectChanges();
      expect(debugEl.nativeElement.textContent).toContain('New Task');

      // Fill and submit
      debugEl.query(By.css('#title')).nativeElement.value = 'New Test Task';
      debugEl.query(By.css('#title')).nativeElement.dispatchEvent(new Event('input'));
      debugEl.query(By.css('button[type="submit"]')).nativeElement.click();
      fixture.detectChanges();

      httpController.expectOne('http://localhost:3000/api/tasks').flush({
        id: '5',
        title: 'New Test Task',
        description: '',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      });
      fixture.detectChanges();

      expect(debugEl.query(By.css('app-task-form'))).toBeNull();
      expect(debugEl.nativeElement.textContent).toContain('New Test Task');
    });

    it('opens form for editing and updates task on submit', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([
        { id: '1', title: 'Original Task', description: '', status: 'todo', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ]);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;

      // Click edit button
      debugEl.query(By.css('.btn-icon[title="Edit"]')).nativeElement.click();
      fixture.detectChanges();

      expect(debugEl.nativeElement.textContent).toContain('Edit Task');
      expect(debugEl.query(By.css('#title')).nativeElement.value).toBe('Original Task');

      // Update and submit
      debugEl.query(By.css('#title')).nativeElement.value = 'Updated Task';
      debugEl.query(By.css('#title')).nativeElement.dispatchEvent(new Event('input'));
      debugEl.query(By.css('button[type="submit"]')).nativeElement.click();
      fixture.detectChanges();

      const req = httpController.expectOne('http://localhost:3000/api/tasks/1');
      expect(req.request.method).toBe('PATCH');
      req.flush({
        id: '1',
        title: 'Updated Task',
        description: '',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      });
      fixture.detectChanges();

      expect(debugEl.nativeElement.textContent).toContain('Updated Task');
      expect(debugEl.nativeElement.textContent).toContain('Total: 1'); // No duplicate
    });

    it('clears edit context when opening new task form after editing', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([
        { id: '1', title: 'Task', description: '', status: 'todo', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ]);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;

      // Open edit, close, then open new
      debugEl.query(By.css('.btn-icon[title="Edit"]')).nativeElement.click();
      fixture.detectChanges();
      debugEl.query(By.css('.modal-overlay')).nativeElement.click();
      fixture.detectChanges();
      debugEl.query(By.css('.btn-add')).nativeElement.click();
      fixture.detectChanges();

      expect(debugEl.nativeElement.textContent).toContain('New Task');
      expect(debugEl.query(By.css('#title')).nativeElement.value).toBe('');
    });
  });

  describe('Task Operations', () => {
    it('onFormSubmit calls addTask for new tasks', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([]);
      fixture.detectChanges();

      component.onFormSubmit({ title: 'New Task', description: 'Desc', priority: 'high' });

      const req = httpController.expectOne('http://localhost:3000/api/tasks');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ title: 'New Task', description: 'Desc', priority: 'high' });
    });

    it('onFormSubmit calls updateTask when editing', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([
        { id: '1', title: 'Task', status: 'todo', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ]);
      fixture.detectChanges();

      component.editingTask.set({ id: '1', title: 'Task', description: '', status: 'todo', priority: 'medium', archived: false, createdAt: new Date(), updatedAt: new Date() });
      component.onFormSubmit({ title: 'Updated', description: 'New desc', priority: 'low' });

      const req = httpController.expectOne('http://localhost:3000/api/tasks/1');
      expect(req.request.method).toBe('PATCH');
      expect(req.request.body).toEqual({ title: 'Updated', description: 'New desc', priority: 'low' });
    });

    it('moves task to new status via drag and drop', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([
        { id: '1', title: 'Task', status: 'todo', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ]);
      fixture.detectChanges();

      component.onTaskMoved({ taskId: '1', newStatus: 'in-progress' });

      const req = httpController.expectOne('http://localhost:3000/api/tasks/1');
      expect(req.request.method).toBe('PATCH');
      expect(req.request.body).toEqual({ status: 'in-progress' });
    });

    it('deletes task', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([
        { id: '1', title: 'Task', status: 'todo', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ]);
      fixture.detectChanges();

      component.onTaskDeleted('1');

      const req = httpController.expectOne('http://localhost:3000/api/tasks/1');
      expect(req.request.method).toBe('DELETE');
    });

    it('archives task', () => {
      httpController.expectOne('http://localhost:3000/api/tasks').flush([
        { id: '1', title: 'Task', status: 'done', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ]);
      fixture.detectChanges();

      component.onTaskArchived('1');

      const req = httpController.expectOne('http://localhost:3000/api/tasks/1');
      expect(req.request.method).toBe('PATCH');
      expect(req.request.body).toEqual({ archived: true });
    });
  });
});
