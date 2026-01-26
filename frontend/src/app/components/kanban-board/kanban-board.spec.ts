import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';
import { KanbanBoardComponent } from './kanban-board';

const API_URL = 'http://localhost:3000/api/tasks';

describe('KanbanBoardComponent', () => {
  let component: KanbanBoardComponent;
  let fixture: ComponentFixture<KanbanBoardComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KanbanBoardComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(KanbanBoardComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('creates', () => {
    httpMock.expectOne(API_URL).flush([]);
    expect(component).toBeTruthy();
  });

  it('renders three columns with correct titles', () => {
    httpMock.expectOne(API_URL).flush([]);
    fixture.detectChanges();

    const columns = fixture.debugElement.queryAll(By.css('app-task-column'));
    expect(columns.length).toBe(3);

    const headers = fixture.debugElement.queryAll(By.css('.column-header h2'));
    expect(headers[0].nativeElement.textContent).toContain('To Do');
    expect(headers[1].nativeElement.textContent).toContain('In Progress');
    expect(headers[2].nativeElement.textContent).toContain('Done');
  });

  it('displays task counts in header', () => {
    httpMock.expectOne(API_URL).flush([
      { id: '1', title: 'Task 1', status: 'todo', priority: 'high', archived: false, createdAt: '2024-01-01', updatedAt: '2024-01-01' },
      { id: '2', title: 'Task 2', status: 'todo', priority: 'medium', archived: false, createdAt: '2024-01-01', updatedAt: '2024-01-01' },
      { id: '3', title: 'Task 3', status: 'in-progress', priority: 'low', archived: false, createdAt: '2024-01-01', updatedAt: '2024-01-01' },
      { id: '4', title: 'Task 4', status: 'done', priority: 'high', archived: false, createdAt: '2024-01-01', updatedAt: '2024-01-01' },
      { id: '5', title: 'Task 5', status: 'done', priority: 'medium', archived: true, createdAt: '2024-01-01', updatedAt: '2024-01-01' }
    ]);
    fixture.detectChanges();

    const stats = fixture.debugElement.query(By.css('.header-stats')).nativeElement.textContent;
    expect(stats).toContain('Total: 4');
    expect(stats).toContain('To Do: 2');
    expect(stats).toContain('In Progress: 1');
    expect(stats).toContain('Done: 1');
  });

  it('opens task form when add button is clicked', () => {
    httpMock.expectOne(API_URL).flush([]);
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('app-task-form'))).toBeNull();

    fixture.debugElement.query(By.css('.btn-add')).nativeElement.click();
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('app-task-form'))).not.toBeNull();
  });
});
