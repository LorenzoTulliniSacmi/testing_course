import { inputBinding, outputBinding, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { vi } from 'vitest';
import { Task, TaskPriority, TaskStatus } from '../../models/task.model';
import { TaskCardComponent } from './task-card';

function createMockTask(overrides: Partial<Task> = {}): Task {
  return {
    id: '1',
    title: 'Test Task',
    description: 'Test description',
    status: 'todo' as TaskStatus,
    priority: 'medium' as TaskPriority,
    archived: false,
    createdAt: new Date('2026-01-01'),
    updatedAt: new Date('2026-01-01'),
    ...overrides
  };
}
const mockTask = createMockTask();
const taskSignal = signal<Task>(mockTask);
let editedTask: Task | null = null;
let deletedTaskId: string | null = null;
let archivedTaskId: string | null = null;

describe('TaskCardComponent', () => {
  let fixture: ComponentFixture<TaskCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskCardComponent, {
      bindings: [
        inputBinding('task', taskSignal),
        outputBinding('edit', (task: Task) => { editedTask = task; }),
        outputBinding('delete', (taskId: string) => { deletedTaskId = taskId; }),
        outputBinding('archive', (taskId: string) => { archivedTaskId = taskId; }),
      ]
    });
    await fixture.whenStable();
  });

  afterEach(() => {
    editedTask = null;
    deletedTaskId = null;
    archivedTaskId = null;
  });

  describe('Rendering', () => {
    it('displays task title, description, and priority', () => {
      const card = fixture.debugElement.query(By.css('.task-card'));
      expect(card.nativeElement.textContent).toContain(mockTask.title);
      expect(card.nativeElement.textContent).toContain(mockTask.description);

      const badge = fixture.debugElement.query(By.css('.priority-badge'));
      expect(badge.nativeElement.textContent).toContain(mockTask.priority);
    });
  });

  describe('Drag and Drop', () => {
    it('sets task id in dataTransfer on dragstart and has draggable attribute', () => {

      const card = fixture.debugElement.query(By.css('.task-card'));
      expect(card.nativeElement.getAttribute('draggable')).toBe('true');

      let transferredData = '';
      const dragStartEvent = new Event('dragstart', { bubbles: true, cancelable: true });
      Object.defineProperty(dragStartEvent, 'dataTransfer', {
        value: { setData: (_type: string, data: string) => { transferredData = data; } }
      });

      card.nativeElement.dispatchEvent(dragStartEvent);
      expect(transferredData).toBe(mockTask.id);
    });
  });

  describe('Output Events', () => {
    it('emits edit event with task when edit button is clicked', () => {
      const editButton = fixture.debugElement.query(By.css('.btn-icon[title="Edit"]'));
      editButton.nativeElement.click();

      expect(editedTask).toEqual(mockTask);
    });

    it('emits delete event only when confirmation is accepted', () => {
      const deleteButton = fixture.debugElement.query(By.css('.btn-icon[title="Delete"]'));

      // Cancelled confirmation
      vi.spyOn(window, 'confirm').mockReturnValue(false);
      deleteButton.nativeElement.click();
      expect(deletedTaskId).toBeNull();

      // Accepted confirmation
      vi.spyOn(window, 'confirm').mockReturnValue(true);
      deleteButton.nativeElement.click();
      expect(deletedTaskId).toBe(mockTask.id);

      vi.restoreAllMocks();
    });

    it('emits archive event when archive button is clicked', () => {
      const archiveButton = fixture.debugElement.query(By.css('.btn-icon[title="Archive"]'));
      if (archiveButton) {
        archiveButton.nativeElement.click();
        expect(archivedTaskId).toBe(mockTask.id);
      }
    });
  });

  // EXERCISE TESTS - Students implement these methods
  // describe('getPriorityClass (Exercise 8)', () => {
  //   let component: TaskCardComponent;

  //   beforeEach(() => {
  //     const taskCardDebug = fixture.debugElement.query(By.directive(TaskCardComponent));
  //     component = taskCardDebug.componentInstance;
  //   });

  //   it.skip('returns correct CSS class for each priority level', () => {
  //     hostComponent.task.set(createMockTask({ priority: 'high' }));
  //     fixture.detectChanges();
  //     expect(component.getPriorityClass()).toBe('priority-high');

  //     hostComponent.task.set(createMockTask({ priority: 'medium' }));
  //     fixture.detectChanges();
  //     expect(component.getPriorityClass()).toBe('priority-medium');

  //     hostComponent.task.set(createMockTask({ priority: 'low' }));
  //     fixture.detectChanges();
  //     expect(component.getPriorityClass()).toBe('priority-low');
  //   });
  // });

  // describe('isStale (Exercise 9)', () => {
  //   let component: TaskCardComponent;

  //   beforeEach(() => {
  //     const taskCardDebug = fixture.debugElement.query(By.directive(TaskCardComponent));
  //     component = taskCardDebug.componentInstance;
  //   });

  //   it.skip('returns true for tasks older than 7 days, false otherwise', () => {
  //     // Task updated today - not stale
  //     hostComponent.task.set(createMockTask({ updatedAt: new Date() }));
  //     fixture.detectChanges();
  //     expect(component.isStale()).toBe(false);

  //     // Task updated 6 days ago - not stale
  //     const sixDaysAgo = new Date();
  //     sixDaysAgo.setDate(sixDaysAgo.getDate() - 6);
  //     hostComponent.task.set(createMockTask({ updatedAt: sixDaysAgo }));
  //     fixture.detectChanges();
  //     expect(component.isStale()).toBe(false);

  //     // Task updated 8 days ago - stale
  //     const eightDaysAgo = new Date();
  //     eightDaysAgo.setDate(eightDaysAgo.getDate() - 8);
  //     hostComponent.task.set(createMockTask({ updatedAt: eightDaysAgo }));
  //     fixture.detectChanges();
  //     expect(component.isStale()).toBe(true);
  //   });
  // });
});
