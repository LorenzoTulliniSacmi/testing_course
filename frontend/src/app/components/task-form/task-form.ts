import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TitleCasePipe } from '@angular/common';
import { Task, TaskPriority } from '../../models/task.model';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [ReactiveFormsModule, TitleCasePipe],
  templateUrl: './task-form.html',
  styleUrl: './task-form.scss'
})
export class TaskFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private taskService = inject(TaskService);

  task = input<Task | null>(null);
  formClose = output<void>();

  form!: FormGroup;
  isSubmitting = signal(false);

  priorities: TaskPriority[] = ['low', 'medium', 'high'];

  ngOnInit(): void {
    this.form = this.fb.group({
      title: [this.task()?.title ?? '', [Validators.required, Validators.minLength(3)]],
      description: [this.task()?.description ?? ''],
      priority: [this.task()?.priority ?? 'medium', Validators.required]
    });
  }

  get isEditMode(): boolean {
    return this.task() !== null;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    const { title, description, priority } = this.form.value;

    if (this.isEditMode) {
      this.taskService.updateTask(this.task()!.id, { title, description, priority });
    } else {
      this.taskService.addTask(title, description, priority);
    }

    this.formClose.emit();
    this.isSubmitting.set(false);
  }

  onCancel(): void {
    this.formClose.emit();
  }

  // EXERCISE 12: Implement custom validator
  // Create a validator that prevents titles containing "test" (case-insensitive)
  // This is for demonstration purposes only
  // Hint: Return { forbiddenTitle: true } if invalid, null if valid
}
