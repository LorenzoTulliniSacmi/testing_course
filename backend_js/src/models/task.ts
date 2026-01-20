export type TaskStatus = 'todo' | 'in-progress' | 'done';

export type TaskPriority = 'low' | 'medium' | 'high';

export interface Task {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  archived: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskDto {
  title: string;
  description: string;
  priority?: TaskPriority;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  archived?: boolean;
}

export type PatchTaskDto = Partial<UpdateTaskDto>;

export interface TaskQueryParams {
  status?: TaskStatus;
  priority?: TaskPriority;
  archived?: string;
  search?: string;
  orderBy?: 'createdAt' | 'updatedAt' | 'priority' | 'title';
  order?: 'asc' | 'desc';
}
