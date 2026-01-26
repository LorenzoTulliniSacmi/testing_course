import { inputBinding, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { Task, TaskStatus, createMockTask } from '../../models/task.model';
import { TaskColumnComponent, TaskMoveData } from './task-column';

function mockDragEvent(type: string, data: string): DragEvent {
  const event = new Event(type, { bubbles: true, cancelable: true }) as any;
  event.dataTransfer = {
    getData: () => data
  };
  event.preventDefault = () => {};
  return event;
}

describe('TaskColumnComponent', () => {
  let fixture: ComponentFixture<TaskColumnComponent>;

  const statusSignal = signal<TaskStatus>('todo');
  const titleSignal = signal('To Do');
  const tasksSignal = signal<Task[]>([]);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskColumnComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskColumnComponent, {
      bindings: [
        inputBinding('status', statusSignal),
        inputBinding('title', titleSignal),
        inputBinding('tasks', tasksSignal)
      ]
    });
    await fixture.whenStable();
  });

  it('creates', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('displays column title and task count', () => {
    titleSignal.set('In Progress');
    tasksSignal.set([
      createMockTask({ id: '1' }),
      createMockTask({ id: '2' }),
      createMockTask({ id: '3' })
    ]);
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.column-header h2')).nativeElement.textContent).toContain('In Progress');
    expect(fixture.debugElement.query(By.css('.task-count')).nativeElement.textContent).toContain('3');
  });

  it('shows empty state when no tasks', () => {
    tasksSignal.set([]);
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.empty-state')).nativeElement.textContent).toContain('No tasks yet');
    expect(fixture.debugElement.queryAll(By.css('app-task-card')).length).toBe(0);
  });

  it('emits taskMove with correct status on drop', () => {
    statusSignal.set('in-progress');
    fixture.detectChanges();

    let emittedMove: TaskMoveData | null = null;
    fixture.componentInstance.taskMove.subscribe((data: TaskMoveData) => emittedMove = data);

    const column = fixture.debugElement.query(By.css('.column')).nativeElement;
    const dropEvent = mockDragEvent('drop', 'task-456');
    column.dispatchEvent(dropEvent);

    expect(emittedMove).toEqual({ taskId: 'task-456', newStatus: 'in-progress' });
  });
});
