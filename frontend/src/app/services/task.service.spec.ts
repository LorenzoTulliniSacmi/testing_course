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

  describe('Computed Signals', () => {
    it('filters active tasks and computes counts correctly', () => {
      const tasks: Task[] = [
        createMockTask({ id: '1', status: 'todo' }),
        createMockTask({ id: '2', status: 'todo' }),
        createMockTask({ id: '3', status: 'in-progress' }),
        createMockTask({ id: '4', status: 'done' }),
        createMockTask({ id: '5', status: 'done', archived: true })
      ];
      service.setTasks(tasks);

      const activeTasks = service.activeTasks();
      expect(activeTasks.length).toBe(4); // archived excluded
      expect(activeTasks.filter(t => t.status === 'todo').length).toBe(2);
      expect(activeTasks.filter(t => t.status === 'in-progress').length).toBe(1);
      expect(activeTasks.filter(t => t.status === 'done').length).toBe(1);

      const counts = service.taskCounts();
      expect(counts.total).toBe(4); // archived excluded
      expect(counts.todo).toBe(2);
      expect(counts.inProgress).toBe(1);
      expect(counts.done).toBe(1);
    });
  });

  describe('CRUD Operations', () => {
    it('adds a new task via API and updates state', () => {
      service.addTask('New Task', 'Description', 'high');

      const req = httpController.expectOne(API_URL);
      expect(req.request.method).toBe('POST');
      req.flush({
        id: '2',
        title: 'New Task',
        description: 'Description',
        status: 'todo',
        priority: 'high',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      });

      expect(service.taskCounts().total).toBe(1);
      expect(service.activeTasks()[0].title).toBe('New Task');
    });

    it('updates an existing task via API', () => {
      service.setTasks([createMockTask({ id: '1', title: 'Original' })]);

      service.updateTask('1', { title: 'Updated' });

      const req = httpController.expectOne(`${API_URL}/1`);
      expect(req.request.method).toBe('PATCH');
      req.flush({
        id: '1',
        title: 'Updated',
        description: 'Test description',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      });

      expect(service.getTaskById('1')?.title).toBe('Updated');
    });

    it('deletes a task via API and removes from state', () => {
      service.setTasks([
        createMockTask({ id: '1' }),
        createMockTask({ id: '2' })
      ]);

      service.deleteTask('1');

      const req = httpController.expectOne(`${API_URL}/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);

      expect(service.taskCounts().total).toBe(1);
      expect(service.getTaskById('1')).toBeUndefined();
      expect(service.getTaskById('2')).toBeDefined();
    });

    it('moves a task to a new status', () => {
      service.setTasks([createMockTask({ id: '1', status: 'todo' })]);

      service.moveTask('1', 'done');

      const req = httpController.expectOne(`${API_URL}/1`);
      req.flush({
        id: '1',
        title: 'Test Task',
        description: 'Test description',
        status: 'done',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      });

      expect(service.getTaskById('1')?.status).toBe('done');
      expect(service.activeTasks().filter(t => t.status === 'todo').length).toBe(0);
      expect(service.activeTasks().filter(t => t.status === 'done').length).toBe(1);
    });

    it('archives a task and removes it from active view', () => {
      service.setTasks([createMockTask({ id: '1', status: 'done', archived: false })]);

      service.archiveTask('1');

      httpController.expectOne(`${API_URL}/1`).flush({
        id: '1',
        title: 'Test Task',
        description: 'Test description',
        status: 'done',
        priority: 'medium',
        archived: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-02T00:00:00Z'
      });

      expect(service.taskCounts().total).toBe(0);
      expect(service.activeTasks().length).toBe(0);
    });
  });

  describe('getTaskById', () => {
    it('returns task when found, undefined when not found, and includes archived tasks', () => {
      service.setTasks([
        createMockTask({ id: '1', title: 'Active' }),
        createMockTask({ id: '2', archived: true })
      ]);

      expect(service.getTaskById('1')?.title).toBe('Active');
      expect(service.getTaskById('2')).toBeDefined(); // archived still accessible
      expect(service.getTaskById('nonexistent')).toBeUndefined();
    });
  });

  describe('Error Handling', () => {
    it('does not update state when API calls fail', () => {
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
      service.setTasks([createMockTask({ id: '1', title: 'Original' })]);

      // Failed update
      service.updateTask('1', { title: 'Updated' });
      httpController.expectOne(`${API_URL}/1`).error(new ProgressEvent('error'));

      expect(service.getTaskById('1')?.title).toBe('Original');

      // Failed delete
      service.deleteTask('1');
      httpController.expectOne(`${API_URL}/1`).error(new ProgressEvent('error'));

      expect(service.taskCounts().total).toBe(1);

      consoleSpy.mockRestore();
    });
  });

  describe('clearAll', () => {
    it('removes all tasks from state', () => {
      service.setTasks([createMockTask({ id: '1' }), createMockTask({ id: '2' })]);

      service.clearAll();

      expect(service.taskCounts().total).toBe(0);
      expect(service.activeTasks().length).toBe(0);
    });
  });
});
