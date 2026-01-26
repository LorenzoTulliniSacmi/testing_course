import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { TaskFormComponent } from './task-form';
import { createMockTask, TaskFormData } from '../../models/task.model';

describe('TaskFormComponent', () => {
  let fixture: ComponentFixture<TaskFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskFormComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskFormComponent);
    await fixture.whenStable();
  });

  it('creates', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('shows validation error when submitting empty title', () => {
    fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement.click();
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.error'));
    expect(error.nativeElement.textContent).toContain('Title is required');
  });

  it('emits form data when form is valid', () => {
    let emittedData: TaskFormData | null = null;
    fixture.componentInstance.formSubmit.subscribe((data: TaskFormData) => emittedData = data);

    const titleInput = fixture.debugElement.query(By.css('#title')).nativeElement;
    titleInput.value = 'New Task Title';
    titleInput.dispatchEvent(new Event('input'));

    const prioritySelect = fixture.debugElement.query(By.css('#priority')).nativeElement;
    prioritySelect.value = 'high';
    prioritySelect.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement.click();
    fixture.detectChanges();

    expect(emittedData).toEqual({
      title: 'New Task Title',
      description: '',
      priority: 'high'
    });
  });

  it('pre-fills form fields in edit mode', () => {
    const task = createMockTask({ title: 'Existing Task', description: 'Some description', priority: 'low' });
    fixture.componentRef.setInput('task', task);
    fixture.componentInstance.ngOnInit();
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.modal-header h2')).nativeElement.textContent).toContain('Edit Task');
    expect(fixture.debugElement.query(By.css('#title')).nativeElement.value).toBe('Existing Task');
    expect(fixture.debugElement.query(By.css('#description')).nativeElement.value).toBe('Some description');
    expect(fixture.debugElement.query(By.css('#priority')).nativeElement.value).toBe('low');
  });
});
