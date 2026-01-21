import { Component, input, output } from '@angular/core';
import type { Task, TaskStatus } from '../../models/task.model';
import { TaskCardComponent } from '../task-card/task-card';

export interface TaskMoveData {
  taskId: string;
  newStatus: TaskStatus;
}

@Component({
  selector: 'app-task-column',
  imports: [TaskCardComponent],
  templateUrl: './task-column.html',
  styleUrl: './task-column.scss'
})
export class TaskColumnComponent {
  status = input.required<TaskStatus>();
  title = input.required<string>();
  tasks = input.required<Task[]>();

  taskEdit = output<Task>();
  taskMove = output<TaskMoveData>();
  taskDelete = output<string>();
  taskArchive = output<string>();

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    (event.currentTarget as HTMLElement).classList.add('drag-over');
  }

  onDragLeave(event: DragEvent): void {
    (event.currentTarget as HTMLElement).classList.remove('drag-over');
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    (event.currentTarget as HTMLElement).classList.remove('drag-over');

    const taskId = event.dataTransfer?.getData('text/plain');
    if (taskId) {
      this.taskMove.emit({ taskId, newStatus: this.status() });
    }
  }

  onTaskEdit(task: Task): void {
    this.taskEdit.emit(task);
  }

  onTaskDelete(taskId: string): void {
    this.taskDelete.emit(taskId);
  }

  onTaskArchive(taskId: string): void {
    this.taskArchive.emit(taskId);
  }
}
