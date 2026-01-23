import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-archive',
  imports: [RouterLink],
  templateUrl: './archive.html',
  styleUrl: './archive.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ArchiveComponent {
  private taskService = inject(TaskService);

  archivedTasks = this.taskService.archivedTasks;

  onUnarchive(taskId: string): void {
    this.taskService.unarchiveTask(taskId);
  }

  onDelete(taskId: string): void {
    if (confirm('Are you sure you want to permanently delete this task?')) {
      this.taskService.deleteTask(taskId);
    }
  }
}
