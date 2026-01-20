import { readFileSync, writeFileSync, existsSync } from 'fs';
import { join } from 'path';
import { v4 as uuidv4 } from 'uuid';
import { Task, CreateTaskDto, UpdateTaskDto, TaskQueryParams, TaskPriority } from '../models/task.js';
import { TaskRepository } from './task.repository.js';

const DATA_FILE = join(process.cwd(), 'data', 'tasks.json');

const PRIORITY_ORDER: Record<TaskPriority, number> = {
  high: 3,
  medium: 2,
  low: 1,
};

export class JsonTaskRepository implements TaskRepository {
  private readTasks(): Task[] {
    try {
      if (!existsSync(DATA_FILE)) {
        return [];
      }
      const data = readFileSync(DATA_FILE, 'utf-8');
      return JSON.parse(data);
    } catch {
      return [];
    }
  }

  private writeTasks(tasks: Task[]): void {
    writeFileSync(DATA_FILE, JSON.stringify(tasks, null, 2));
  }

  async findAll(query: TaskQueryParams): Promise<Task[]> {
    let tasks = this.readTasks();
    const { status, priority, archived, search, orderBy, order } = query;

    // Filter by archived (default: show non-archived)
    const showArchived = archived === 'true';
    const showAll = archived === 'all';
    if (!showAll) {
      tasks = tasks.filter((t) => t.archived === showArchived);
    }

    // Filter by status
    if (status) {
      tasks = tasks.filter((t) => t.status === status);
    }

    // Filter by priority
    if (priority) {
      tasks = tasks.filter((t) => t.priority === priority);
    }

    // Search by title (case-insensitive)
    if (search) {
      const searchLower = search.toLowerCase();
      tasks = tasks.filter((t) => t.title.toLowerCase().includes(searchLower));
    }

    // Ordering
    if (orderBy) {
      const sortOrder = order === 'desc' ? -1 : 1;
      tasks.sort((a, b) => {
        if (orderBy === 'priority') {
          return (PRIORITY_ORDER[a.priority] - PRIORITY_ORDER[b.priority]) * sortOrder;
        }
        if (orderBy === 'createdAt' || orderBy === 'updatedAt') {
          return (new Date(a[orderBy]).getTime() - new Date(b[orderBy]).getTime()) * sortOrder;
        }
        if (orderBy === 'title') {
          return a.title.localeCompare(b.title) * sortOrder;
        }
        return 0;
      });
    }

    return tasks;
  }

  async findById(id: string): Promise<Task | null> {
    const tasks = this.readTasks();
    return tasks.find((t) => t.id === id) || null;
  }

  async create(dto: CreateTaskDto): Promise<Task> {
    const tasks = this.readTasks();
    const now = new Date().toISOString();

    const newTask: Task = {
      id: uuidv4(),
      title: dto.title,
      description: dto.description,
      status: 'todo',
      priority: dto.priority || 'medium',
      archived: false,
      createdAt: now,
      updatedAt: now,
    };

    tasks.push(newTask);
    this.writeTasks(tasks);

    return newTask;
  }

  async update(id: string, dto: UpdateTaskDto): Promise<Task | null> {
    const tasks = this.readTasks();
    const index = tasks.findIndex((t) => t.id === id);

    if (index === -1) {
      return null;
    }

    const task = tasks[index];
    const updatedTask: Task = {
      ...task,
      ...dto,
      updatedAt: new Date().toISOString(),
    };

    tasks[index] = updatedTask;
    this.writeTasks(tasks);

    return updatedTask;
  }

  async delete(id: string): Promise<boolean> {
    const tasks = this.readTasks();
    const index = tasks.findIndex((t) => t.id === id);

    if (index === -1) {
      return false;
    }

    tasks.splice(index, 1);
    this.writeTasks(tasks);

    return true;
  }
}
