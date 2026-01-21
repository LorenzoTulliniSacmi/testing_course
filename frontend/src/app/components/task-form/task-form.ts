import { Component, inject, input, output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TitleCasePipe } from '@angular/common';
import { Task, TaskFormData, TaskPriority } from '../../models/task.model';

@Component({
  selector: 'app-task-form',
  imports: [ReactiveFormsModule, TitleCasePipe],
  templateUrl: './task-form.html',
  styleUrl: './task-form.scss'
})
export class TaskFormComponent implements OnInit {
  private fb = inject(FormBuilder);

  task = input<Task | null>(null);
  formSubmit = output<TaskFormData>();
  formClose = output<void>();

  form!: FormGroup;

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

    this.formSubmit.emit(this.form.value as TaskFormData);
    this.formClose.emit();
  }

  onCancel(): void {
    this.formClose.emit();
  }

  // EXERCISE 12: Implement custom validator
  // Create a validator that prevents titles containing "test" (case-insensitive)
  // This is for demonstration purposes only
  // Hint: Return { forbiddenTitle: true } if invalid, null if valid
}
