import { Component, inject, signal } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { TaskColumnComponent } from '../task-column/task-column';
import { TaskFormComponent } from '../task-form/task-form';
import { Task, TaskFormData, TaskStatus, ColumnConfig } from '../../models/task.model';

@Component({
  selector: 'app-kanban-board',
  imports: [TaskColumnComponent, TaskFormComponent],
  templateUrl: './kanban-board.html',
  styleUrl: './kanban-board.scss'
})
export class KanbanBoardComponent {
  private taskService = inject(TaskService);

  readonly columns: readonly ColumnConfig[] = [
    { id: 'todo', title: 'To Do' },
    { id: 'in-progress', title: 'In Progress' },
    { id: 'done', title: 'Done' }
  ];

  activeTasks = this.taskService.activeTasks;
  taskCounts = this.taskService.taskCounts;

  tasksForStatus(status: TaskStatus): Task[] {
    return this.activeTasks().filter(task => task.status === status);
  }

  showTaskForm = signal(false);
  editingTask = signal<Task | null>(null);

  openNewTaskForm(): void {
    this.editingTask.set(null);
    this.showTaskForm.set(true);
  }

  openEditTaskForm(task: Task): void {
    this.editingTask.set(task);
    this.showTaskForm.set(true);
  }

  closeTaskForm(): void {
    this.showTaskForm.set(false);
    this.editingTask.set(null);
  }

  onFormSubmit(data: TaskFormData): void {
    const task = this.editingTask();
    if (task) {
      this.taskService.updateTask(task.id, data);
    } else {
      this.taskService.addTask(data.title, data.description, data.priority);
    }
  }

  onTaskMoved(event: { taskId: string; newStatus: TaskStatus }): void {
    this.taskService.moveTask(event.taskId, event.newStatus);
  }

  onTaskDeleted(taskId: string): void {
    this.taskService.deleteTask(taskId);
  }

  onTaskArchived(taskId: string): void {
    this.taskService.archiveTask(taskId);
  }
}
