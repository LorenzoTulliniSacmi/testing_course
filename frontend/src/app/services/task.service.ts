import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Task, TaskStatus, TaskPriority } from '../models/task.model';

const API_URL = 'http://localhost:3000/api/tasks';

interface ApiTask {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  archived: boolean;
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private http = inject(HttpClient);
  private tasks = signal<Task[]>([]);

  // Computed signal for active (non-archived) tasks
  activeTasks = computed(() => this.tasks().filter(t => !t.archived));

  // Computed signal for task counts (excludes archived)
  taskCounts = computed(() => {
    const activeTasks = this.tasks().filter(t => !t.archived);
    return {
      total: activeTasks.length,
      todo: activeTasks.filter(t => t.status === 'todo').length,
      inProgress: activeTasks.filter(t => t.status === 'in-progress').length,
      done: activeTasks.filter(t => t.status === 'done').length
    };
  });

  constructor() {
    this.loadTasks();
  }

  private mapApiTask(apiTask: ApiTask): Task {
    return {
      ...apiTask,
      createdAt: new Date(apiTask.createdAt),
      updatedAt: new Date(apiTask.updatedAt)
    };
  }

  loadTasks(): void {
    this.http.get<ApiTask[]>(API_URL).subscribe({
      next: (apiTasks) => {
        const tasks = apiTasks.map(t => this.mapApiTask(t));
        this.tasks.set(tasks);
      },
      error: (err) => console.error('Failed to load tasks:', err)
    });
  }

  addTask(title: string, description: string, priority: TaskPriority = 'medium'): void {
    this.http.post<ApiTask>(API_URL, { title, description, priority }).subscribe({
      next: (apiTask) => {
        const task = this.mapApiTask(apiTask);
        this.tasks.update(tasks => [...tasks, task]);
      },
      error: (err) => console.error('Failed to add task:', err)
    });
  }

  updateTask(id: string, updates: Partial<Omit<Task, 'id' | 'createdAt'>>): void {
    this.http.patch<ApiTask>(`${API_URL}/${id}`, updates).subscribe({
      next: (apiTask) => {
        const updatedTask = this.mapApiTask(apiTask);
        this.tasks.update(tasks =>
          tasks.map(task => task.id === id ? updatedTask : task)
        );
      },
      error: (err) => console.error('Failed to update task:', err)
    });
  }

  deleteTask(id: string): void {
    this.http.delete(`${API_URL}/${id}`).subscribe({
      next: () => {
        this.tasks.update(tasks => tasks.filter(task => task.id !== id));
      },
      error: (err) => console.error('Failed to delete task:', err)
    });
  }

  moveTask(taskId: string, newStatus: TaskStatus): void {
    this.updateTask(taskId, { status: newStatus });
  }

  archiveTask(id: string): void {
    this.http.patch<ApiTask>(`${API_URL}/${id}`, { archived: true }).subscribe({
      next: (apiTask) => {
        const updatedTask = this.mapApiTask(apiTask);
        this.tasks.update(tasks =>
          tasks.map(task => task.id === id ? updatedTask : task)
        );
      },
      error: (err) => console.error('Failed to archive task:', err)
    });
  }

  getTaskById(id: string): Task | undefined {
    return this.tasks().find(task => task.id === id);
  }

  // EXERCISE 1: Implement this method
  // Filter tasks by priority - returns tasks matching the given priority
  // Hint: Use the tasks signal and filter
  filterByPriority(priority: TaskPriority): Task[] {
    // TODO: Implement this method
    throw new Error('Not implemented - Exercise 1');
  }

  // EXERCISE 2: Implement this method
  // Search tasks by title (case-insensitive)
  // Hint: Use toLowerCase() for case-insensitive comparison
  searchTasks(query: string): Task[] {
    // TODO: Implement this method
    throw new Error('Not implemented - Exercise 2');
  }

  // For testing purposes - clears all tasks
  clearAll(): void {
    this.tasks.set([]);
  }

  // For testing purposes - set tasks directly
  setTasks(tasks: Task[]): void {
    this.tasks.set(tasks);
  }
}
