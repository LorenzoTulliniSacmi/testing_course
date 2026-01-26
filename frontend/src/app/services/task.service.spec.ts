import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { TaskService } from './task.service';
import { createMockTask } from '../models/task.model';
import { create } from 'node:domain';

const API_URL = 'http://localhost:3000/api/tasks';

describe('TaskService', () => {
  let service: TaskService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TaskService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(TaskService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('creates and loads tasks on init', () => {
    httpMock.expectOne(API_URL).flush([
      createMockTask({ id: '1', archived: false })
    ]);

    expect(service).toBeTruthy();
    expect(service.activeTasks().length).toBe(1);
  });

  it('filters archived tasks from activeTasks and computes counts', () => {
    httpMock.expectOne(API_URL).flush([
      createMockTask({ id: '1', title: 'Todo', status: 'todo', priority: 'high', archived: false }),
      createMockTask({ id: '2', title: 'In Progress', status: 'in-progress', priority: 'medium', archived: false }),
      createMockTask({ id: '3', title: 'Done', status: 'done', priority: 'low', archived: false }),
      createMockTask({ id: '4', title: 'Archived', status: 'done', priority: 'high', archived: true })
    ]);

    expect(service.activeTasks().length).toBe(3);
    expect(service.taskCounts()).toEqual({ total: 3, todo: 1, inProgress: 1, done: 1 });
  });

  it('adds task via POST and updates state', () => {
    httpMock.expectOne(API_URL).flush([]);

    service.addTask('New Task', 'Description', 'high');

    const req = httpMock.expectOne(API_URL);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'New Task', description: 'Description', priority: 'high' });

    req.flush({ id: '1', title: 'New Task', description: 'Description', status: 'todo', priority: 'high', archived: false, createdAt: '2024-01-01', updatedAt: '2024-01-01' });

    expect(service.activeTasks().length).toBe(1);
    expect(service.activeTasks()[0].title).toBe('New Task');
  });

  it('deletes task via DELETE and removes from state', () => {
    httpMock.expectOne(API_URL).flush([createMockTask({ id: '1', archived: false })]);

    service.deleteTask('1');

    const req = httpMock.expectOne(`${API_URL}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);

    expect(service.activeTasks().length).toBe(0);
  });
});
