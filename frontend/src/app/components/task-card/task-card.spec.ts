import { inputBinding, outputBinding, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { Task, createMockTask } from '../../models/task.model';
import { TaskCardComponent } from './task-card';

describe('TaskCardComponent', () => {
  let fixture: ComponentFixture<TaskCardComponent>;
  const taskSignal = signal<Task>(createMockTask());

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskCardComponent, {
      bindings: [inputBinding('task', taskSignal)]
    });
    await fixture.whenStable();
  });

  it('creates', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('displays task title, description, and priority badge', () => {
    taskSignal.set(createMockTask({
      title: 'My Task',
      description: 'Task details here',
      priority: 'high'
    }));
    fixture.detectChanges();

    const card = fixture.debugElement.query(By.css('.task-card'));
    expect(card.nativeElement.textContent).toContain('My Task');
    expect(card.nativeElement.textContent).toContain('Task details here');
    expect(fixture.debugElement.query(By.css('.priority-badge')).nativeElement.textContent).toContain('high');
  });

  it('emits edit event with task when edit button is clicked', () => {
    const task = createMockTask({ id: 'task-123', title: 'Editable Task' });
    taskSignal.set(task);
    fixture.detectChanges();

    let emittedTask: Task | null = null;
    fixture.componentInstance.edit.subscribe((t: Task) => emittedTask = t);

    fixture.debugElement.query(By.css('.btn-icon[title="Edit"]')).nativeElement.click();

    expect(emittedTask).toEqual(task);
  });

  it('shows archive button only for tasks with done status', () => {
    taskSignal.set(createMockTask({ status: 'todo' }));
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('.btn-icon[title="Archive"]'))).toBeNull();

    taskSignal.set(createMockTask({ status: 'done' }));
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('.btn-icon[title="Archive"]'))).not.toBeNull();
  });
});
