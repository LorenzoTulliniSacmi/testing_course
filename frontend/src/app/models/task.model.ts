export type TaskStatus = 'todo' | 'in-progress' | 'done';

export type TaskPriority = 'low' | 'medium' | 'high';

export interface Task {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  archived: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface ColumnConfig {
  readonly id: TaskStatus;
  readonly title: string;
}

export interface TaskFormData {
  title: string;
  description: string;
  priority: TaskPriority;
}

export const COLUMN_TITLES: Record<TaskStatus, string> = {
  'todo': 'To Do',
  'in-progress': 'In Progress',
  'done': 'Done'
};


export function createMockTask(overrides: Partial<Task> = {}): Task {
  return {
    id: 'default-id',
    title: 'Default Task',
    description: 'Default description',
    status: 'todo',
    priority: 'medium',
    archived: false,
    createdAt: new Date(),
    updatedAt: new Date(),
    ...overrides
  };
}
