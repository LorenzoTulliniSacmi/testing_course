import { ComponentFixture, TestBed } from "@angular/core/testing";
import { KanbanBoardComponent } from "./kanban-board";
import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { By } from "@angular/platform-browser";

const mockTasks = [
  { id: '1', title: 'Todo Task', status: 'todo', priority: 'high', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' },
  { id: '2', title: 'In Progress Task', status: 'in-progress', priority: 'medium', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' },
  { id: '3', title: 'Done Task', status: 'done', priority: 'low', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' },
  { id: '4', title: 'Done and archived Task', status: 'done', priority: 'high', archived: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
];

describe('Kanban Board Component', () => {
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

  it('displays the Kanban board with columns', () => {
    httpController.expectOne('http://localhost:3000/api/tasks').flush([]);

    const debugEl = fixture.debugElement
    expect(debugEl.queryAll(By.css('app-task-column')).length).toBeGreaterThan(0);
  });

  it('renders tasks in their correct columns', () => {
    httpController.expectOne('http://localhost:3000/api/tasks').flush(mockTasks);
    fixture.detectChanges();

    const debugEl = fixture.debugElement;
    const columns = debugEl.queryAll(By.css('app-task-column'));

    expect(columns.length).toBe(3);
    expect(columns[0].nativeElement.textContent).toContain('Todo Task');
    expect(columns[1].nativeElement.textContent).toContain('In Progress Task');
    expect(columns[2].nativeElement.textContent).toContain('Done Task');
    expect(debugEl.nativeElement.textContent).toContain('Total: 3');
  });

  it('hides archived tasks from the board', () => {
    httpController.expectOne('http://localhost:3000/api/tasks').flush(mockTasks);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;

    // Archived task should not appear anywhere on the board
    expect(compiled.textContent).not.toContain('Done and archived Task');

    // Task counts should only include active (non-archived) tasks
    expect(compiled.textContent).toContain('Total: 3');
    expect(compiled.textContent).toContain('Done: 1');
  });

  it('opens task form when clicking Add Task button', () => {
    httpController.expectOne('http://localhost:3000/api/tasks').flush([]);
    fixture.detectChanges();

    const compiled = fixture.debugElement;
    const addButton = compiled.query(By.css('.btn-add'));

    expect(compiled.query(By.css('app-task-form'))).toBeNull();

    addButton.nativeElement.click();
    fixture.detectChanges();

    expect(compiled.query(By.css('app-task-form'))).not.toBeNull();
    expect(compiled.nativeElement.textContent).toContain('New Task');
  });

  it('creates a new task via the form', () => {
    httpController.expectOne('http://localhost:3000/api/tasks').flush([]);
    fixture.detectChanges();

    const compiled = fixture.debugElement;

    // Open the form
    const addButton = compiled.query(By.css('.btn-add'));
    addButton.nativeElement.click();
    fixture.detectChanges();

    // Fill out the form
    const titleInput = compiled.query(By.css('#title'));
    const descriptionInput = compiled.query(By.css('#description'));
    const prioritySelect = compiled.query(By.css('#priority'));

    titleInput.nativeElement.value = 'New Integration Test Task';
    titleInput.nativeElement.dispatchEvent(new Event('input'));
    descriptionInput.nativeElement.value = 'Task description';
    descriptionInput.nativeElement.dispatchEvent(new Event('input'));
    prioritySelect.nativeElement.value = 'high';
    prioritySelect.nativeElement.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    // Submit the form
    const submitButton = compiled.query(By.css('button[type="submit"]'));
    submitButton.nativeElement.click();
    fixture.detectChanges();

    // Mock the POST response
    const newTask = {
      id: '5',
      title: 'New Integration Test Task',
      description: 'Task description',
      status: 'todo',
      priority: 'high',
      archived: false,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z'
    };
    httpController.expectOne('http://localhost:3000/api/tasks').flush(newTask);
    fixture.detectChanges();

    // Form should be closed and task should appear in To Do column
    expect(compiled.query(By.css('app-task-form'))).toBeNull();
    expect(compiled.nativeElement.textContent).toContain('New Integration Test Task');
    expect(compiled.nativeElement.textContent).toContain('Total: 1');
  });
});
