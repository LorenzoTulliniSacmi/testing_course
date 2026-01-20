import { Component, input, output } from '@angular/core';
import { Column, Task, TaskStatus } from '../../models/task.model';
import { TaskCardComponent } from '../task-card/task-card';

@Component({
  selector: 'app-task-column',
  standalone: true,
  imports: [TaskCardComponent],
  templateUrl: './task-column.html',
  styleUrl: './task-column.scss'
})
export class TaskColumnComponent {
  column = input.required<Column>();

  taskEdit = output<Task>();
  taskMove = output<{ taskId: string; newStatus: TaskStatus }>();
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
      this.taskMove.emit({ taskId, newStatus: this.column().id });
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
