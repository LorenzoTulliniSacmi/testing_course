import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TaskFormComponent } from './task-form';
import { By } from '@angular/platform-browser';
import { createMockTask, TaskFormData } from '../../models/task.model';
import { DebugElement } from '@angular/core';

describe('TaskFormComponent', () => {
  let fixture: ComponentFixture<TaskFormComponent>;
  let debugEl: DebugElement;
  let emittedData: TaskFormData | null = null;
  let formClosed = false;

  beforeEach(async () => {
    emittedData = null;
    formClosed = false;

    await TestBed.configureTestingModule({
      imports: [TaskFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskFormComponent);
    debugEl = fixture.debugElement;

    fixture.componentInstance.formSubmit.subscribe((data: TaskFormData) => {
      emittedData = data;
    });
    fixture.componentInstance.formClose.subscribe(() => {
      formClosed = true;
    });

    await fixture.whenStable();
  });

  describe('Form Validation', () => {
    it('shows validation errors for invalid title and prevents submission', () => {
      // Submit empty form
      debugEl.query(By.css('button[type="submit"]')).nativeElement.click();
      fixture.detectChanges();

      expect(debugEl.query(By.css('.error')).nativeElement.textContent).toContain(
        'Title is required',
      );
      expect(emittedData).toBeNull();

      // Title too short
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      titleInput.value = 'ab';
      titleInput.dispatchEvent(new Event('input'));
      debugEl.query(By.css('button[type="submit"]')).nativeElement.click();
      fixture.detectChanges();

      expect(debugEl.query(By.css('.error')).nativeElement.textContent).toContain(
        'at least 3 characters',
      );
      expect(emittedData).toBeNull();
    });

    it('emits form data when valid', () => {
      const titleInput = debugEl.query(By.css('#title')).nativeElement;
      titleInput.value = 'Valid Title';
      titleInput.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      debugEl.query(By.css('button[type="submit"]')).nativeElement.click();
      fixture.detectChanges();

      expect(emittedData).toEqual({
        title: 'Valid Title',
        description: '',
        priority: 'medium',
      });
      expect(formClosed).toBe(true);
    });
  });

  describe('Create vs Edit Mode', () => {
    it('shows "New Task" header and empty fields in create mode', () => {
      expect(debugEl.query(By.css('.modal-header h2')).nativeElement.textContent).toContain(
        'New Task',
      );
      expect(debugEl.query(By.css('#title')).nativeElement.value).toBe('');
    });

    it('shows "Edit Task" header and pre-fills fields in edit mode', () => {
      const task = createMockTask({ title: 'Existing', description: 'Desc', priority: 'high' });
      fixture.componentRef.setInput('task', task);
      fixture.componentInstance.ngOnInit();
      fixture.detectChanges();

      expect(debugEl.query(By.css('.modal-header h2')).nativeElement.textContent).toContain(
        'Edit Task',
      );
      expect(debugEl.query(By.css('#title')).nativeElement.value).toBe('Existing');
      expect(debugEl.query(By.css('#description')).nativeElement.value).toBe('Desc');
      expect(debugEl.query(By.css('#priority')).nativeElement.value).toBe('high');
    });
  });

  describe('Form Submission', () => {
    it('emits complete form data with all fields', () => {
      debugEl.query(By.css('#title')).nativeElement.value = 'New Task';
      debugEl.query(By.css('#title')).nativeElement.dispatchEvent(new Event('input'));
      debugEl.query(By.css('#description')).nativeElement.value = 'Description';
      debugEl.query(By.css('#description')).nativeElement.dispatchEvent(new Event('input'));
      debugEl.query(By.css('#priority')).nativeElement.value = 'high';
      debugEl.query(By.css('#priority')).nativeElement.dispatchEvent(new Event('change'));
      fixture.detectChanges();

      debugEl.query(By.css('button[type="submit"]')).nativeElement.click();
      fixture.detectChanges();

      expect(emittedData).toEqual({
        title: 'New Task',
        description: 'Description',
        priority: 'high',
      });
    });
  });

  describe('Close Functionality', () => {
    it('emits formClose on cancel without emitting form data', () => {
      debugEl.query(By.css('#title')).nativeElement.value = 'Some Task';
      debugEl.query(By.css('#title')).nativeElement.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      debugEl.query(By.css('.btn-secondary')).nativeElement.click();
      fixture.detectChanges();

      expect(formClosed).toBe(true);
      expect(emittedData).toBeNull();
    });

    it('emits formClose on overlay click', () => {
      debugEl.query(By.css('.modal-overlay')).nativeElement.click();
      fixture.detectChanges();

      expect(formClosed).toBe(true);
    });
  });
});
