import { Component, inject, signal } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { TaskColumnComponent } from '../task-column/task-column';
import { TaskFormComponent } from '../task-form/task-form';
import { Task, TaskStatus } from '../../models/task.model';

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [TaskColumnComponent, TaskFormComponent],
  templateUrl: './kanban-board.html',
  styleUrl: './kanban-board.scss'
})
export class KanbanBoardComponent {
  private taskService = inject(TaskService);

  columns = this.taskService.columns;
  taskCounts = this.taskService.taskCounts;

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
