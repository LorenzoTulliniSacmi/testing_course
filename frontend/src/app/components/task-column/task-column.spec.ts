import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { By } from '@angular/platform-browser';
import { vi } from 'vitest';
import { TaskColumnComponent } from './task-column';
import { Column, Task, TaskStatus } from '../../models/task.model';

// Helper to create mock drag events (jsdom doesn't have DragEvent)
function createDragEvent(type: string, dataTransfer?: { data: string } | null): Event {
  const event = new Event(type, { bubbles: true, cancelable: true });
  if (dataTransfer !== null) {
    Object.defineProperty(event, 'dataTransfer', {
      value: {
        getData: () => dataTransfer?.data ?? ''
      }
    });
  }
  return event;
}

function createMockTask(overrides: Partial<Task> = {}): Task {
  return {
    id: '1',
    title: 'Test Task',
    description: 'Test description',
    status: 'todo' as TaskStatus,
    priority: 'medium',
    archived: false,
    createdAt: new Date('2024-01-01'),
    updatedAt: new Date('2024-01-01'),
    ...overrides
  };
}

function createMockColumn(overrides: Partial<Column> = {}): Column {
  return {
    id: 'todo' as TaskStatus,
    title: 'To Do',
    tasks: [],
    ...overrides
  };
}

// Test host component to pass inputs and capture outputs
@Component({
  imports: [TaskColumnComponent],
  template: `
    <app-task-column
      [column]="column()"
      (taskEdit)="onTaskEdit($event)"
      (taskMove)="onTaskMove($event)"
      (taskDelete)="onTaskDelete($event)"
      (taskArchive)="onTaskArchive($event)"
    />
  `
})
class TestHostComponent {
  column = signal<Column>(createMockColumn());

  editedTask: Task | null = null;
  movedTask: { taskId: string; newStatus: TaskStatus } | null = null;
  deletedTaskId: string | null = null;
  archivedTaskId: string | null = null;

  onTaskEdit(task: Task): void {
    this.editedTask = task;
  }

  onTaskMove(event: { taskId: string; newStatus: TaskStatus }): void {
    this.movedTask = event;
  }

  onTaskDelete(taskId: string): void {
    this.deletedTaskId = taskId;
  }

  onTaskArchive(taskId: string): void {
    this.archivedTaskId = taskId;
  }
}

describe('TaskColumnComponent', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let hostComponent: TestHostComponent;
  let httpController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    httpController = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(TestHostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();

    // Flush the initial loadTasks request from TaskService (may be triggered by child components)
    const initialRequest = httpController.match('http://localhost:3000/api/tasks');
    initialRequest.forEach(req => req.flush([]));
  });

  afterEach(() => {
    httpController.verify();
  });

  describe('Rendering', () => {
    it('displays the column title', () => {
      hostComponent.column.set(createMockColumn({ title: 'In Progress' }));
      fixture.detectChanges();

      const header = fixture.debugElement.query(By.css('.column-header'));
      expect(header.nativeElement.textContent).toContain('In Progress');
    });

    it('renders task cards for each task in column', () => {
      hostComponent.column.set(createMockColumn({
        tasks: [
          createMockTask({ id: '1', title: 'Task 1' }),
          createMockTask({ id: '2', title: 'Task 2' }),
          createMockTask({ id: '3', title: 'Task 3' })
        ]
      }));
      fixture.detectChanges();

      const taskCards = fixture.debugElement.queryAll(By.css('app-task-card'));
      expect(taskCards.length).toBe(3);
    });

    it('displays empty column when no tasks', () => {
      hostComponent.column.set(createMockColumn({ tasks: [] }));
      fixture.detectChanges();

      const taskCards = fixture.debugElement.queryAll(By.css('app-task-card'));
      expect(taskCards.length).toBe(0);
    });
  });

  describe('Drag and Drop', () => {
    let columnElement: HTMLElement;

    beforeEach(() => {
      hostComponent.column.set(createMockColumn({
        id: 'in-progress',
        title: 'In Progress',
        tasks: []
      }));
      fixture.detectChanges();
      columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;
    });

    it('adds drag-over class on dragover event', () => {
      const dragOverEvent = createDragEvent('dragover');

      columnElement.dispatchEvent(dragOverEvent);

      expect(columnElement.classList.contains('drag-over')).toBe(true);
    });

    it('prevents default on dragover to allow drop', () => {
      const dragOverEvent = createDragEvent('dragover');

      columnElement.dispatchEvent(dragOverEvent);

      expect(dragOverEvent.defaultPrevented).toBe(true);
    });

    it('removes drag-over class on dragleave event', () => {
      // First add the class
      columnElement.classList.add('drag-over');

      const dragLeaveEvent = createDragEvent('dragleave');

      columnElement.dispatchEvent(dragLeaveEvent);

      expect(columnElement.classList.contains('drag-over')).toBe(false);
    });

    it('emits taskMove on drop with correct taskId and newStatus', () => {
      const dropEvent = createDragEvent('drop', { data: 'task-123' });

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).toEqual({
        taskId: 'task-123',
        newStatus: 'in-progress'
      });
    });

    it('removes drag-over class on drop', () => {
      columnElement.classList.add('drag-over');

      const dropEvent = createDragEvent('drop', { data: 'task-123' });

      columnElement.dispatchEvent(dropEvent);

      expect(columnElement.classList.contains('drag-over')).toBe(false);
    });

    it('prevents default on drop event', () => {
      const dropEvent = createDragEvent('drop', { data: 'task-123' });

      columnElement.dispatchEvent(dropEvent);

      expect(dropEvent.defaultPrevented).toBe(true);
    });

    it('does not emit taskMove if dataTransfer is empty', () => {
      const dropEvent = createDragEvent('drop', { data: '' });

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).toBeNull();
    });

    it('does not emit taskMove if dataTransfer is undefined', () => {
      const dropEvent = createDragEvent('drop', null);

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).toBeNull();
    });
  });

  describe('Drag from To Do to In Progress', () => {
    it('moves task from To Do to In Progress column', () => {
      // Setup target column (In Progress)
      hostComponent.column.set(createMockColumn({
        id: 'in-progress',
        title: 'In Progress',
        tasks: []
      }));
      fixture.detectChanges();

      const columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;

      // Simulate drop with task ID that came from To Do
      const dropEvent = createDragEvent('drop', { data: 'todo-task-1' });

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).toEqual({
        taskId: 'todo-task-1',
        newStatus: 'in-progress'
      });
    });
  });

  describe('Output Events', () => {
    beforeEach(() => {
      hostComponent.column.set(createMockColumn({
        tasks: [createMockTask({ id: 'task-1', title: 'Test Task' })]
      }));
      fixture.detectChanges();
    });

    it('emits taskEdit when task card emits edit', () => {
      const taskCard = fixture.debugElement.query(By.css('app-task-card'));
      const editButton = taskCard.query(By.css('.btn-icon[title="Edit"]'));

      editButton.nativeElement.click();
      fixture.detectChanges();

      expect(hostComponent.editedTask).toBeDefined();
      expect(hostComponent.editedTask?.id).toBe('task-1');
    });

    it('emits taskDelete when task card emits delete', () => {
      // Mock confirm dialog
      vi.spyOn(window, 'confirm').mockReturnValue(true);

      const taskCard = fixture.debugElement.query(By.css('app-task-card'));
      const deleteButton = taskCard.query(By.css('.btn-icon[title="Delete"]'));

      deleteButton.nativeElement.click();
      fixture.detectChanges();

      expect(hostComponent.deletedTaskId).toBe('task-1');

      vi.restoreAllMocks();
    });

    it('does not emit taskDelete when confirm is cancelled', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(false);

      const taskCard = fixture.debugElement.query(By.css('app-task-card'));
      const deleteButton = taskCard.query(By.css('.btn-icon[title="Delete"]'));

      deleteButton.nativeElement.click();
      fixture.detectChanges();

      expect(hostComponent.deletedTaskId).toBeNull();

      vi.restoreAllMocks();
    });
  });

  describe('Archive Functionality', () => {
    it('emits taskArchive for done column tasks', () => {
      hostComponent.column.set(createMockColumn({
        id: 'done',
        title: 'Done',
        tasks: [createMockTask({ id: 'done-task', status: 'done' })]
      }));
      fixture.detectChanges();

      const taskCard = fixture.debugElement.query(By.css('app-task-card'));
      const archiveButton = taskCard.query(By.css('.btn-icon[title="Archive"]'));

      if (archiveButton) {
        archiveButton.nativeElement.click();
        fixture.detectChanges();

        expect(hostComponent.archivedTaskId).toBe('done-task');
      }
    });
  });

  describe('Drop Updates Computed Signals', () => {
    it('emits correct status for todo column', () => {
      hostComponent.movedTask = null;
      hostComponent.column.set(createMockColumn({ id: 'todo' }));
      fixture.detectChanges();

      const columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;
      const dropEvent = createDragEvent('drop', { data: 'some-task-id' });

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).not.toBeNull();
      expect(hostComponent.movedTask!.newStatus).toBe('todo');
    });

    it('emits correct status for in-progress column', () => {
      hostComponent.movedTask = null;
      hostComponent.column.set(createMockColumn({ id: 'in-progress' }));
      fixture.detectChanges();

      const columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;
      const dropEvent = createDragEvent('drop', { data: 'some-task-id' });

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).not.toBeNull();
      expect(hostComponent.movedTask!.newStatus).toBe('in-progress');
    });

    it('emits correct status for done column', () => {
      hostComponent.movedTask = null;
      hostComponent.column.set(createMockColumn({ id: 'done' }));
      fixture.detectChanges();

      const columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;
      const dropEvent = createDragEvent('drop', { data: 'some-task-id' });

      columnElement.dispatchEvent(dropEvent);

      expect(hostComponent.movedTask).not.toBeNull();
      expect(hostComponent.movedTask!.newStatus).toBe('done');
    });
  });
});
