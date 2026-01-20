import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TaskFormComponent } from "./task-form";
import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { By } from "@angular/platform-browser";
import { Component, signal } from "@angular/core";
import { Task } from "../../models/task.model";

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
});
