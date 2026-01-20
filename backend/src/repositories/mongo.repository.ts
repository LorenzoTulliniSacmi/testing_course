import mongoose, { Schema, Document } from 'mongoose';
import { Task, CreateTaskDto, UpdateTaskDto, TaskQueryParams, TaskPriority } from '../models/task.js';
import { TaskRepository } from './task.repository.js';

interface TaskDocument extends Omit<Task, 'id'>, Document {}

const taskSchema = new Schema<TaskDocument>(
  {
    title: { type: String, required: true },
    description: { type: String, required: true },
    status: { type: String, enum: ['todo', 'in-progress', 'done'], default: 'todo' },
    priority: { type: String, enum: ['low', 'medium', 'high'], default: 'medium' },
    archived: { type: Boolean, default: false },
  },
  {
    timestamps: true,
    toJSON: {
      virtuals: true,
      transform: (_, ret: Record<string, unknown>) => {
        ret.id = ret._id?.toString();
        ret._id = undefined;
        ret.__v = undefined;
        return ret;
      },
    },
  }
);

const TaskModel = mongoose.model<TaskDocument>('Task', taskSchema);

const PRIORITY_ORDER: Record<TaskPriority, number> = {
  high: 3,
  medium: 2,
  low: 1,
};

export class MongoTaskRepository implements TaskRepository {
  async connect(uri: string): Promise<void> {
    await mongoose.connect(uri);
    console.log('Connected to MongoDB');
  }

  async findAll(query: TaskQueryParams): Promise<Task[]> {
    const { status, priority, archived, search, orderBy, order } = query;

    const filter: Record<string, unknown> = {};

    // Filter by archived (default: show non-archived)
    const showAll = archived === 'all';
    if (!showAll) {
      filter.archived = archived === 'true';
    }

    // Filter by status
    if (status) {
      filter.status = status;
    }

    // Filter by priority
    if (priority) {
      filter.priority = priority;
    }

    // Search by title (case-insensitive)
    if (search) {
      filter.title = { $regex: search, $options: 'i' };
    }

    let mongoQuery = TaskModel.find(filter);

    // Ordering
    if (orderBy) {
      const sortOrder = order === 'desc' ? -1 : 1;
      if (orderBy === 'priority') {
        // For priority, we need to sort in memory after fetching
        const tasks = await mongoQuery.lean().exec();
        tasks.sort((a, b) => {
          const aPriority = PRIORITY_ORDER[a.priority as TaskPriority];
          const bPriority = PRIORITY_ORDER[b.priority as TaskPriority];
          return (aPriority - bPriority) * sortOrder;
        });
        return tasks.map(this.toTask);
      } else {
        const sortField = orderBy === 'createdAt' ? 'createdAt' : orderBy === 'updatedAt' ? 'updatedAt' : 'title';
        mongoQuery = mongoQuery.sort({ [sortField]: sortOrder });
      }
    }

    const tasks = await mongoQuery.lean().exec();
    return tasks.map(this.toTask);
  }

  async findById(id: string): Promise<Task | null> {
    if (!mongoose.Types.ObjectId.isValid(id)) {
      return null;
    }
    const task = await TaskModel.findById(id).lean().exec();
    return task ? this.toTask(task) : null;
  }

  async create(dto: CreateTaskDto): Promise<Task> {
    const task = new TaskModel({
      title: dto.title,
      description: dto.description,
      priority: dto.priority || 'medium',
      status: 'todo',
      archived: false,
    });

    const saved = await task.save();
    return saved.toJSON() as Task;
  }

  async update(id: string, dto: UpdateTaskDto): Promise<Task | null> {
    if (!mongoose.Types.ObjectId.isValid(id)) {
      return null;
    }

    const updated = await TaskModel.findByIdAndUpdate(
      id,
      { ...dto, updatedAt: new Date() },
      { new: true }
    ).lean().exec();

    return updated ? this.toTask(updated) : null;
  }

  async delete(id: string): Promise<boolean> {
    if (!mongoose.Types.ObjectId.isValid(id)) {
      return false;
    }

    const result = await TaskModel.findByIdAndDelete(id).exec();
    return result !== null;
  }

  private toTask(doc: Record<string, unknown>): Task {
    return {
      id: (doc._id as mongoose.Types.ObjectId).toString(),
      title: doc.title as string,
      description: doc.description as string,
      status: doc.status as Task['status'],
      priority: doc.priority as Task['priority'],
      archived: doc.archived as boolean,
      createdAt: (doc.createdAt as Date).toISOString(),
      updatedAt: (doc.updatedAt as Date).toISOString(),
    };
  }
}
