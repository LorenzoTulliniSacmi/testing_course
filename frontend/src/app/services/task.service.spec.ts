import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { vi } from 'vitest';
import { TaskService } from './task.service';
import { Task, TaskPriority, TaskStatus } from '../models/task.model';

const API_URL = 'http://localhost:3000/api/tasks';

function createMockTask(overrides: Partial<Task> = {}): Task {
  return {
    id: '1',
    title: 'Test Task',
    description: 'Test description',
    status: 'todo' as TaskStatus,
    priority: 'medium' as TaskPriority,
    archived: false,
    createdAt: new Date('2024-01-01'),
    updatedAt: new Date('2024-01-01'),
    ...overrides
  };
}

describe('TaskService', () => {
  let service: TaskService;
  let httpController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TaskService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(TaskService);
    httpController = TestBed.inject(HttpTestingController);

    // Flush the initial loadTasks request
    httpController.expectOne(API_URL).flush([]);
  });

  afterEach(() => {
    httpController.verify();
  });

  describe('State Initialization', () => {
    it('starts with empty task list', () => {
      expect(service.columns().length).toBe(3);
      expect(service.taskCounts().total).toBe(0);
    });

    it('loads tasks from API on construction', () => {
      // Force a reload to test the HTTP call
      service.loadTasks();

      const mockTasks = [
        { id: '1', title: 'Task 1', description: '', status: 'todo', priority: 'high', archived: false, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-01T00:00:00Z' }
      ];
      httpController.expectOne(API_URL).flush(mockTasks);

      expect(service.taskCounts().total).toBe(1);
    });
  });

  describe('Computed Signals - columns', () => {
    it('distributes tasks to correct columns', () => {
      const tasks: Task[] = [
        createMockTask({ id: '1', status: 'todo' }),
        createMockTask({ id: '2', status: 'in-progress' }),
        createMockTask({ id: '3', status: 'done' })
      ];
      service.setTasks(tasks);

      const columns = service.columns();
      expect(columns[0].id).toBe('todo');
      expect(columns[0].tasks.length).toBe(1);
      expect(columns[1].id).toBe('in-progress');
      expect(columns[1].tasks.length).toBe(1);
      expect(columns[2].id).toBe('done');
      expect(columns[2].tasks.length).toBe(1);
    });

    it('excludes archived tasks from columns', () => {
      const tasks: Task[] = [
        createMockTask({ id: '1', status: 'done', archived: false }),
        createMockTask({ id: '2', status: 'done', archived: true })
      ];
      service.setTasks(tasks);

      const doneColumn = service.columns().find(c => c.id === 'done');
      expect(doneColumn?.tasks.length).toBe(1);
      expect(doneColumn?.tasks[0].id).toBe('1');
    });

    it('updates columns when tasks are modified', () => {
      const tasks: Task[] = [createMockTask({ id: '1', status: 'todo' })];
      service.setTasks(tasks);

      expect(service.columns()[0].tasks.length).toBe(1);

      service.setTasks([]);
      expect(service.columns()[0].tasks.length).toBe(0);
    });
  });

  describe('Computed Signals - taskCounts', () => {
    it('correctly counts tasks by status', () => {
      const tasks: Task[] = [
        createMockTask({ id: '1', status: 'todo' }),
        createMockTask({ id: '2', status: 'todo' }),
        createMockTask({ id: '3', status: 'in-progress' }),
        createMockTask({ id: '4', status: 'done' })
      ];
      service.setTasks(tasks);

      const counts = service.taskCounts();
      expect(counts.total).toBe(4);
      expect(counts.todo).toBe(2);
      expect(counts.inProgress).toBe(1);
      expect(counts.done).toBe(1);
    });

    it('excludes archived tasks from counts', () => {
      const tasks: Task[] = [
        createMockTask({ id: '1', status: 'todo', archived: false }),
        createMockTask({ id: '2', status: 'todo', archived: true }),
        createMockTask({ id: '3', status: 'done', archived: true })
      ];
      service.setTasks(tasks);

      const counts = service.taskCounts();
      expect(counts.total).toBe(1);
      expect(counts.todo).toBe(1);
      expect(counts.done).toBe(0);
    });
  });

  describe('addTask', () => {
    it('adds task to state after API response', () => {
      service.addTask('New Task', 'Description', 'high');

      const newTaskResponse = {
        id: '2',
        title: 'New Task',
        description: 'Description',
        status: 'todo',
        priority: 'high',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      };
      httpController.expectOne(API_URL).flush(newTaskResponse);

      expect(service.taskCounts().total).toBe(1);
      expect(service.columns()[0].tasks[0].title).toBe('New Task');
    });

    it('preserves existing tasks when adding new task', () => {
      service.setTasks([createMockTask({ id: '1', title: 'Existing' })]);

      service.addTask('New Task', 'Desc', 'medium');

      const newTaskResponse = {
        id: '2',
        title: 'New Task',
        description: 'Desc',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      };
      httpController.expectOne(API_URL).flush(newTaskResponse);

      expect(service.taskCounts().total).toBe(2);
    });
  });

  describe('updateTask', () => {
    it('updates task in state after API response', () => {
      service.setTasks([createMockTask({ id: '1', title: 'Original' })]);

      service.updateTask('1', { title: 'Updated' });

      const updatedResponse = {
        id: '1',
        title: 'Updated',
        description: 'Test description',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      };
      httpController.expectOne(`${API_URL}/1`).flush(updatedResponse);

      const task = service.getTaskById('1');
      expect(task?.title).toBe('Updated');
    });

    it('does not affect other tasks when updating', () => {
      service.setTasks([
        createMockTask({ id: '1', title: 'Task 1' }),
        createMockTask({ id: '2', title: 'Task 2' })
      ]);

      service.updateTask('1', { title: 'Updated' });

      const updatedResponse = {
        id: '1',
        title: 'Updated',
        description: 'Test description',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      };
      httpController.expectOne(`${API_URL}/1`).flush(updatedResponse);

      expect(service.getTaskById('2')?.title).toBe('Task 2');
    });
  });

  describe('deleteTask', () => {
    it('removes task from state after API response', () => {
      service.setTasks([createMockTask({ id: '1' })]);

      service.deleteTask('1');
      httpController.expectOne(`${API_URL}/1`).flush(null);

      expect(service.taskCounts().total).toBe(0);
      expect(service.getTaskById('1')).toBeUndefined();
    });

    it('preserves other tasks when deleting', () => {
      service.setTasks([
        createMockTask({ id: '1' }),
        createMockTask({ id: '2' })
      ]);

      service.deleteTask('1');
      httpController.expectOne(`${API_URL}/1`).flush(null);

      expect(service.taskCounts().total).toBe(1);
      expect(service.getTaskById('2')).toBeDefined();
    });
  });

  describe('moveTask', () => {
    it('updates task status via updateTask', () => {
      service.setTasks([createMockTask({ id: '1', status: 'todo' })]);

      service.moveTask('1', 'in-progress');

      const updatedResponse = {
        id: '1',
        title: 'Test Task',
        description: 'Test description',
        status: 'in-progress',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      };
      httpController.expectOne(`${API_URL}/1`).flush(updatedResponse);

      const task = service.getTaskById('1');
      expect(task?.status).toBe('in-progress');
    });

    it('updates both source and target columns', () => {
      service.setTasks([
        createMockTask({ id: '1', status: 'todo' }),
        createMockTask({ id: '2', status: 'todo' })
      ]);

      service.moveTask('1', 'in-progress');

      const updatedResponse = {
        id: '1',
        title: 'Test Task',
        description: 'Test description',
        status: 'in-progress',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      };
      httpController.expectOne(`${API_URL}/1`).flush(updatedResponse);

      expect(service.columns()[0].tasks.length).toBe(1); // todo
      expect(service.columns()[1].tasks.length).toBe(1); // in-progress
    });
  });

  describe('archiveTask', () => {
    it('archives task and removes from view', () => {
      service.setTasks([createMockTask({ id: '1', status: 'done', archived: false })]);

      service.archiveTask('1');

      const updatedResponse = {
        id: '1',
        title: 'Test Task',
        description: 'Test description',
        status: 'done',
        priority: 'medium',
        archived: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      };
      httpController.expectOne(`${API_URL}/1`).flush(updatedResponse);

      expect(service.taskCounts().total).toBe(0);
      expect(service.columns()[2].tasks.length).toBe(0);
    });

    it('updates both columns and counts when archiving', () => {
      service.setTasks([
        createMockTask({ id: '1', status: 'done', archived: false }),
        createMockTask({ id: '2', status: 'done', archived: false })
      ]);

      service.archiveTask('1');

      const updatedResponse = {
        id: '1',
        title: 'Test Task',
        description: 'Test description',
        status: 'done',
        priority: 'medium',
        archived: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      };
      httpController.expectOne(`${API_URL}/1`).flush(updatedResponse);

      // Both signals should be consistent
      expect(service.taskCounts().done).toBe(1);
      expect(service.columns()[2].tasks.length).toBe(1);
    });
  });

  describe('Multiple Operations Sequence', () => {
    it('maintains state consistency through create, move, edit, archive', () => {
      // Start empty
      expect(service.taskCounts().total).toBe(0);

      // Create task
      service.addTask('Task 1', 'Desc', 'medium');
      httpController.expectOne(API_URL).flush({
        id: '1',
        title: 'Task 1',
        description: 'Desc',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      });

      expect(service.taskCounts().total).toBe(1);
      expect(service.taskCounts().todo).toBe(1);

      // Move to in-progress
      service.moveTask('1', 'in-progress');
      httpController.expectOne(`${API_URL}/1`).flush({
        id: '1',
        title: 'Task 1',
        description: 'Desc',
        status: 'in-progress',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      });

      expect(service.taskCounts().total).toBe(1);
      expect(service.taskCounts().todo).toBe(0);
      expect(service.taskCounts().inProgress).toBe(1);

      // Edit title
      service.updateTask('1', { title: 'Updated Task' });
      httpController.expectOne(`${API_URL}/1`).flush({
        id: '1',
        title: 'Updated Task',
        description: 'Desc',
        status: 'in-progress',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-03T00:00:00Z'
      });

      expect(service.getTaskById('1')?.title).toBe('Updated Task');
      expect(service.taskCounts().total).toBe(1);

      // Move to done
      service.moveTask('1', 'done');
      httpController.expectOne(`${API_URL}/1`).flush({
        id: '1',
        title: 'Updated Task',
        description: 'Desc',
        status: 'done',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-04T00:00:00Z'
      });

      expect(service.taskCounts().done).toBe(1);

      // Archive
      service.archiveTask('1');
      httpController.expectOne(`${API_URL}/1`).flush({
        id: '1',
        title: 'Updated Task',
        description: 'Desc',
        status: 'done',
        priority: 'medium',
        archived: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-05T00:00:00Z'
      });

      expect(service.taskCounts().total).toBe(0);
      expect(service.taskCounts().done).toBe(0);
    });
  });

  describe('getTaskById', () => {
    it('returns task when found', () => {
      service.setTasks([createMockTask({ id: '1', title: 'Found' })]);
      expect(service.getTaskById('1')?.title).toBe('Found');
    });

    it('returns undefined when not found', () => {
      expect(service.getTaskById('nonexistent')).toBeUndefined();
    });

    it('finds archived tasks', () => {
      service.setTasks([createMockTask({ id: '1', archived: true })]);
      expect(service.getTaskById('1')).toBeDefined();
    });
  });

  describe('API Error Handling', () => {
    it('logs error and does not update state on addTask failure', () => {
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

      service.addTask('New Task', 'Desc', 'medium');
      httpController.expectOne(API_URL).error(new ProgressEvent('error'));

      expect(consoleSpy).toHaveBeenCalled();
      expect(service.taskCounts().total).toBe(0);

      consoleSpy.mockRestore();
    });

    it('logs error and does not update state on updateTask failure', () => {
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
      service.setTasks([createMockTask({ id: '1', title: 'Original' })]);

      service.updateTask('1', { title: 'Updated' });
      httpController.expectOne(`${API_URL}/1`).error(new ProgressEvent('error'));

      expect(consoleSpy).toHaveBeenCalled();
      expect(service.getTaskById('1')?.title).toBe('Original');

      consoleSpy.mockRestore();
    });

    it('logs error and does not remove task on deleteTask failure', () => {
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
      service.setTasks([createMockTask({ id: '1' })]);

      service.deleteTask('1');
      httpController.expectOne(`${API_URL}/1`).error(new ProgressEvent('error'));

      expect(consoleSpy).toHaveBeenCalled();
      expect(service.taskCounts().total).toBe(1);

      consoleSpy.mockRestore();
    });
  });

  describe('clearAll', () => {
    it('removes all tasks from state', () => {
      service.setTasks([
        createMockTask({ id: '1' }),
        createMockTask({ id: '2' })
      ]);

      service.clearAll();

      expect(service.taskCounts().total).toBe(0);
      expect(service.columns().every(c => c.tasks.length === 0)).toBe(true);
    });
  });
});
