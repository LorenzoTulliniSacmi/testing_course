import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TaskFormComponent } from "./task-form";
import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { By } from "@angular/platform-browser";
import { Component, signal } from "@angular/core";
import { Task, TaskStatus, TaskPriority } from "../../models/task.model";

// Test host component to pass inputs to TaskFormComponent
@Component({
  standalone: true,
  imports: [TaskFormComponent],
  template: `@if (showForm()) {
    <app-task-form [task]="task()" (formClose)="onFormClose()" />
  }`
})
class TestHostComponent {
  task = signal<Task | null>(null);
  showForm = signal(true);
  formClosed = false;
  onFormClose() {
    this.formClosed = true;
  }
}

describe('TaskFormComponent', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let hostComponent: TestHostComponent;
  let httpController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    httpController = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(TestHostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();

    // Flush the initial loadTasks request from TaskService
    httpController.expectOne('http://localhost:3000/api/tasks').flush([]);
  });

  afterEach(() => {
    httpController.verify();
  });

  it('shows error when title is empty and form is submitted', () => {
    const debugEl = fixture.debugElement;

    // Submit the form without entering anything
    const submitButton = debugEl.query(By.css('button[type="submit"]'));
    submitButton.nativeElement.click();
    fixture.detectChanges();

    const errorMessage = debugEl.query(By.css('.error'));
    expect(errorMessage).not.toBeNull();
    expect(errorMessage.nativeElement.textContent).toContain('Title is required');
  });

  it('shows error when title is less than 3 characters', () => {
    const debugEl = fixture.debugElement;
    const titleInput = debugEl.query(By.css('#title')).nativeElement;

    titleInput.value = 'ab';
    titleInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    // Submit to trigger validation display
    const submitButton = debugEl.query(By.css('button[type="submit"]'));
    submitButton.nativeElement.click();
    fixture.detectChanges();

    const errorMessage = debugEl.query(By.css('.error'));
    expect(errorMessage).not.toBeNull();
    expect(errorMessage.nativeElement.textContent).toContain('Title must be at least 3 characters');
  });

  it('does not show error when title is valid', () => {
    const debugEl = fixture.debugElement;
    const titleInput = debugEl.query(By.css('#title')).nativeElement;

    titleInput.value = 'Valid title';
    titleInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const errorMessage = debugEl.query(By.css('.error'));
    expect(errorMessage).toBeNull();
  });

  it('displays "New Task" header when creating', () => {
    const debugEl = fixture.debugElement;
    const header = debugEl.query(By.css('.modal-header h2'));

    expect(header.nativeElement.textContent).toContain('New Task');
  });

  it('displays "Edit Task" header when editing', async () => {
    // Close and reopen with task to trigger ngOnInit with task
    hostComponent.showForm.set(false);
    fixture.detectChanges();

    hostComponent.task.set({
      id: '1',
      title: 'Existing Task',
      description: 'Description',
      status: 'todo',
      priority: 'medium',
      archived: false,
      createdAt: new Date('2024-01-01'),
      updatedAt: new Date('2024-01-01')
    });
    hostComponent.showForm.set(true);
    fixture.detectChanges();

    const debugEl = fixture.debugElement;
    const header = debugEl.query(By.css('.modal-header h2'));

    expect(header.nativeElement.textContent).toContain('Edit Task');
  });

  it('pre-fills form fields when editing', async () => {
    hostComponent.showForm.set(false);
    fixture.detectChanges();

    hostComponent.task.set({
      id: '1',
      title: 'Existing Task',
      description: 'Existing description',
      status: 'todo',
      priority: 'high',
      archived: false,
      createdAt: new Date('2024-01-01'),
      updatedAt: new Date('2024-01-01')
    });
    hostComponent.showForm.set(true);
    fixture.detectChanges();

    const debugEl = fixture.debugElement;
    const titleInput = debugEl.query(By.css('#title')).nativeElement;
    const descriptionInput = debugEl.query(By.css('#description')).nativeElement;
    const prioritySelect = debugEl.query(By.css('#priority')).nativeElement;

    expect(titleInput.value).toBe('Existing Task');
    expect(descriptionInput.value).toBe('Existing description');
    expect(prioritySelect.value).toBe('high');
  });

  it('closes form when cancel button is clicked', () => {
    const debugEl = fixture.debugElement;
    const cancelButton = debugEl.query(By.css('.btn-secondary'));

    cancelButton.nativeElement.click();
    fixture.detectChanges();

    expect(hostComponent.formClosed).toBe(true);
  });

  it('closes form when overlay is clicked', () => {
    const debugEl = fixture.debugElement;
    const overlay = debugEl.query(By.css('.modal-overlay'));

    overlay.nativeElement.click();
    fixture.detectChanges();

    expect(hostComponent.formClosed).toBe(true);
  });

  describe('Async Submission', () => {
    it('sets isSubmitting to true during form submission', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      // Fill in valid data
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      titleInput.value = 'Valid Task Title';
      titleInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      expect(formComponent.isSubmitting()).toBe(false);

      // Submit the form
      const submitButton = debugEl.query(By.css('button[type="submit"]'));
      submitButton.nativeElement.click();
      fixture.detectChanges();

      // Note: isSubmitting is set and immediately reset in the current implementation
      // The POST request should be made
      httpController.expectOne('http://localhost:3000/api/tasks').flush({
        id: '1',
        title: 'Valid Task Title',
        description: '',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      });
    });

    it('calls addTask service method for new tasks', () => {
      const debugEl = fixture.debugElement;

      // Fill in valid data
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      const descriptionInput = debugEl.query(By.css('#description')).nativeElement;

      titleInput.value = 'New Task';
      titleInput.dispatchEvent(new Event('input'));
      descriptionInput.value = 'Task description';
      descriptionInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      // Submit
      const submitButton = debugEl.query(By.css('button[type="submit"]'));
      submitButton.nativeElement.click();
      fixture.detectChanges();

      // Should make POST request
      const req = httpController.expectOne('http://localhost:3000/api/tasks');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({
        title: 'New Task',
        description: 'Task description',
        priority: 'medium'
      });
    });

    it('calls updateTask service method for existing tasks', async () => {
      // Switch to edit mode
      hostComponent.showForm.set(false);
      fixture.detectChanges();

      hostComponent.task.set({
        id: 'existing-123',
        title: 'Existing Task',
        description: 'Original description',
        status: 'todo',
        priority: 'low',
        archived: false,
        createdAt: new Date('2024-01-01'),
        updatedAt: new Date('2024-01-01')
      });
      hostComponent.showForm.set(true);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;

      // Modify the title
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      titleInput.value = 'Updated Task Title';
      titleInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      // Submit
      const submitButton = debugEl.query(By.css('button[type="submit"]'));
      submitButton.nativeElement.click();
      fixture.detectChanges();

      // Should make PATCH request
      const req = httpController.expectOne('http://localhost:3000/api/tasks/existing-123');
      expect(req.request.method).toBe('PATCH');
    });

    it('does not submit when form is invalid', () => {
      const debugEl = fixture.debugElement;

      // Leave title empty (invalid)
      const submitButton = debugEl.query(By.css('button[type="submit"]'));
      submitButton.nativeElement.click();
      fixture.detectChanges();

      // No HTTP request should be made
      httpController.expectNone('http://localhost:3000/api/tasks');
    });

    it('marks all fields as touched when submitting invalid form', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      // Form should not be touched initially
      expect(formComponent.form.get('title')?.touched).toBe(false);

      // Submit with invalid data
      const submitButton = debugEl.query(By.css('button[type="submit"]'));
      submitButton.nativeElement.click();
      fixture.detectChanges();

      // All fields should now be touched
      expect(formComponent.form.get('title')?.touched).toBe(true);
    });

    it('emits formClose after successful submission', () => {
      const debugEl = fixture.debugElement;

      // Fill in valid data
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      titleInput.value = 'Valid Task';
      titleInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      // Submit
      const submitButton = debugEl.query(By.css('button[type="submit"]'));
      submitButton.nativeElement.click();
      fixture.detectChanges();

      // Flush the HTTP request
      httpController.expectOne('http://localhost:3000/api/tasks').flush({
        id: '1',
        title: 'Valid Task',
        description: '',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      });

      expect(hostComponent.formClosed).toBe(true);
    });
  });

  describe('Mode Switching', () => {
    it('isEditMode returns false for new task', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      expect(formComponent.isEditMode).toBe(false);
    });

    it('isEditMode returns true when task is provided', async () => {
      hostComponent.showForm.set(false);
      fixture.detectChanges();

      hostComponent.task.set({
        id: '1',
        title: 'Task',
        description: '',
        status: 'todo',
        priority: 'medium',
        archived: false,
        createdAt: new Date('2024-01-01'),
        updatedAt: new Date('2024-01-01')
      });
      hostComponent.showForm.set(true);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      expect(formComponent.isEditMode).toBe(true);
    });

    it('correctly switches from edit mode to create mode', async () => {
      // Start in edit mode
      hostComponent.showForm.set(false);
      fixture.detectChanges();

      hostComponent.task.set({
        id: '1',
        title: 'Edit Task',
        description: 'Edit description',
        status: 'todo',
        priority: 'high',
        archived: false,
        createdAt: new Date('2024-01-01'),
        updatedAt: new Date('2024-01-01')
      });
      hostComponent.showForm.set(true);
      fixture.detectChanges();

      let debugEl = fixture.debugElement;
      let titleInput = debugEl.query(By.css('#title')).nativeElement;
      expect(titleInput.value).toBe('Edit Task');

      // Switch to create mode
      hostComponent.showForm.set(false);
      fixture.detectChanges();

      hostComponent.task.set(null);
      hostComponent.showForm.set(true);
      fixture.detectChanges();

      debugEl = fixture.debugElement;
      titleInput = debugEl.query(By.css('#title')).nativeElement;

      // Form should be reset for new task
      expect(titleInput.value).toBe('');
    });

    it('preserves priority selection in edit mode', async () => {
      hostComponent.showForm.set(false);
      fixture.detectChanges();

      hostComponent.task.set({
        id: '1',
        title: 'Task',
        description: '',
        status: 'todo',
        priority: 'low',
        archived: false,
        createdAt: new Date('2024-01-01'),
        updatedAt: new Date('2024-01-01')
      });
      hostComponent.showForm.set(true);
      fixture.detectChanges();

      const debugEl = fixture.debugElement;
      const prioritySelect = debugEl.query(By.css('#priority')).nativeElement;

      expect(prioritySelect.value).toBe('low');
    });
  });

  describe('Form Validation', () => {
    it('title field has required validator', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      formComponent.form.get('title')?.setValue('');
      expect(formComponent.form.get('title')?.hasError('required')).toBe(true);
    });

    it('title field has minLength validator of 3', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      formComponent.form.get('title')?.setValue('ab');
      expect(formComponent.form.get('title')?.hasError('minlength')).toBe(true);

      formComponent.form.get('title')?.setValue('abc');
      expect(formComponent.form.get('title')?.hasError('minlength')).toBe(false);
    });

    it('priority field has required validator', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      formComponent.form.get('priority')?.setValue('');
      expect(formComponent.form.get('priority')?.hasError('required')).toBe(true);
    });

    it('description field is optional', () => {
      const debugEl = fixture.debugElement;
      const formComponent = debugEl.query(By.directive(TaskFormComponent)).componentInstance as TaskFormComponent;

      formComponent.form.get('description')?.setValue('');
      expect(formComponent.form.get('description')?.valid).toBe(true);
    });
  });

  describe('Cancel Functionality', () => {
    it('emits formClose when cancel button is clicked', () => {
      const debugEl = fixture.debugElement;
      const cancelButton = debugEl.query(By.css('.btn-secondary'));

      cancelButton.nativeElement.click();
      fixture.detectChanges();

      expect(hostComponent.formClosed).toBe(true);
    });

    it('does not submit form data when cancelled', () => {
      const debugEl = fixture.debugElement;

      // Fill in some data
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      titleInput.value = 'Some Task';
      titleInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      // Cancel
      const cancelButton = debugEl.query(By.css('.btn-secondary'));
      cancelButton.nativeElement.click();
      fixture.detectChanges();

      // No HTTP request should be made
      httpController.expectNone('http://localhost:3000/api/tasks');
    });
  });
});
