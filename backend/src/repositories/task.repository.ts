import { Task, CreateTaskDto, UpdateTaskDto, TaskQueryParams } from '../models/task.js';

export interface TaskRepository {
  findAll(query: TaskQueryParams): Promise<Task[]>;
  findById(id: string): Promise<Task | null>;
  create(dto: CreateTaskDto): Promise<Task>;
  update(id: string, dto: UpdateTaskDto): Promise<Task | null>;
  delete(id: string): Promise<boolean>;
}
