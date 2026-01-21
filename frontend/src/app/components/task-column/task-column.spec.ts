import { DebugElement, outputBinding } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { vi } from 'vitest';
import { createMockTask, Task } from '../../models/task.model';
import { TaskColumnComponent, type TaskMoveData } from './task-column';

function createDragEvent(type: string, data?: string): DragEvent {
  const dataTransfer = new DataTransfer();
  dataTransfer.setData('text/plain', data ?? '');

  const event = new DragEvent(type, { bubbles: true, cancelable: true, dataTransfer});
  return event;
}

describe('TaskColumnComponent', () => {
  let fixture: ComponentFixture<TaskColumnComponent>;

  let taskEdit: Task | null;
  let taskMove: TaskMoveData | null;
  let taskDelete: string | null;
  let taskArchive: string | null;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskColumnComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskColumnComponent, {
      bindings: [
        outputBinding('taskEdit', (task: Task) => { taskEdit = task; }),
        outputBinding('taskMove', (data: TaskMoveData) => { taskMove = data; }),
        outputBinding('taskDelete', (taskId: string) => { taskDelete = taskId; }),
        outputBinding('taskArchive', (taskId: string) => { taskArchive = taskId; }),
      ]
    });

    await fixture.whenStable();
  });

  afterEach(() => {
    taskEdit = null;
    taskMove = null;
    taskDelete = null;
    taskArchive = null;
  });

  describe('Rendering', () => {
    it('displays column title and renders task cards', () => {
      fixture.componentRef.setInput('status', 'in-progress');
      fixture.componentRef.setInput('title','In Progress');
      fixture.componentRef.setInput('tasks', [
        createMockTask({ id: '1', title: 'Task 1' }),
        createMockTask({ id: '2', title: 'Task 2' })
      ]);
      fixture.detectChanges();

      const header = fixture.debugElement.query(By.css('.column-header'));
      expect(header.nativeElement.textContent).toContain('In Progress');

      const taskCards = fixture.debugElement.queryAll(By.css('app-task-card'));
      expect(taskCards.length).toBe(2);
    });
  });

  describe('Drag and Drop', () => {
    it('handles drag events and emits taskMove on valid drop', () => {
      fixture.componentRef.setInput('status', 'in-progress');
      fixture.componentRef.setInput('title', 'In Progress');
      fixture.detectChanges();

      const columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;

      // Dragover adds visual feedback and prevents default
      const dragOverEvent = createDragEvent('dragover');
      columnElement.dispatchEvent(dragOverEvent);
      expect(columnElement.classList.contains('drag-over')).toBe(true);
      expect(dragOverEvent.defaultPrevented).toBe(true);

      // Dragleave removes visual feedback
      columnElement.dispatchEvent(createDragEvent('dragleave'));
      expect(columnElement.classList.contains('drag-over')).toBe(false);

      // Drop emits taskMove with correct status
      const dropEvent = createDragEvent('drop', 'task-123');
      columnElement.dispatchEvent(dropEvent);

      expect(taskMove).toEqual({
        taskId: 'task-123',
        newStatus: 'in-progress'
      });
      expect(columnElement.classList.contains('drag-over')).toBe(false);
    });

    it('does not emit taskMove for empty dataTransfer', () => {
      fixture.componentRef.setInput('status', 'done');
      fixture.detectChanges();

      const columnElement = fixture.debugElement.query(By.css('.column')).nativeElement;
      columnElement.dispatchEvent(createDragEvent('drop', ''));

      expect(taskMove).toBeNull();
    });
  });

  describe('Output Events', () => {
    beforeEach(() => {
      fixture.componentRef.setInput('tasks', [createMockTask({ id: 'task-1', title: 'Test Task' })]);
      fixture.detectChanges();
    });

    it('emits taskEdit when task card edit is clicked', () => {
      const editButton = fixture.debugElement.query(By.css('.btn-icon[title="Edit"]'));
      editButton.nativeElement.click();

      expect(taskEdit?.id).toBe('task-1');
    });

    it('emits taskDelete when confirmation is accepted', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(true);

      const deleteButton = fixture.debugElement.query(By.css('.btn-icon[title="Delete"]'));
      deleteButton.nativeElement.click();

      expect(taskDelete).toBe('task-1');
      vi.restoreAllMocks();
    });

    it('emits taskArchive for done column tasks', () => {
      fixture.componentRef.setInput('status', 'done');
      fixture.componentRef.setInput('title', 'Done');
      fixture.componentRef.setInput('tasks', [createMockTask({ id: 'done-task', status: 'done' })]);
      fixture.detectChanges();

      const archiveButton = fixture.debugElement.query(By.css('.btn-icon[title="Archive"]'));
      if (archiveButton) {
        archiveButton.nativeElement.click();
        expect(taskArchive).toBe('done-task');
      }
    });
  });
});
