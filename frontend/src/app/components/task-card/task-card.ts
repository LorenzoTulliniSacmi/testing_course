import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './task-card.html',
  styleUrl: './task-card.scss'
})
export class TaskCardComponent {
  task = input.required<Task>();

  edit = output<Task>();
  delete = output<string>();
  archive = output<string>();

  onDragStart(event: DragEvent): void {
    event.dataTransfer?.setData('text/plain', this.task().id);
  }

  onEdit(): void {
    this.edit.emit(this.task());
  }

  onDelete(): void {
    if (confirm('Are you sure you want to delete this task?')) {
      this.delete.emit(this.task().id);
    }
  }

  onArchive(): void {
    this.archive.emit(this.task().id);
  }

  // EXERCISE 8: Implement this method
  // Returns a CSS class based on priority: 'priority-high', 'priority-medium', 'priority-low'
  getPriorityClass(): string {
    // TODO: Implement this method
    return '';
  }

  // EXERCISE 9: Implement this method
  // Returns true if task was updated more than 7 days ago
  isStale(): boolean {
    // TODO: Implement this method
    // Hint: Compare updatedAt with current date
    return false;
  }
}
