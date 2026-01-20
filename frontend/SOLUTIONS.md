# Angular Testing Course - Solutions

**⚠️ INSTRUCTOR ONLY - Do not distribute to students**

This document contains complete solutions for all 18 exercises.

---

## Exercise 1: Implement `filterByPriority`
**File:** `src/app/services/task.service.ts`

```typescript
filterByPriority(priority: TaskPriority): Task[] {
  return this.tasks().filter(task => task.priority === priority);
}
```

---

## Exercise 2: Implement `searchTasks`
**File:** `src/app/services/task.service.ts`

```typescript
searchTasks(query: string): Task[] {
  const lowerQuery = query.toLowerCase();
  return this.tasks().filter(task => 
    task.title.toLowerCase().includes(lowerQuery)
  );
}
```

---

## Exercise 3: Tests for `filterByPriority`
**File:** `src/app/services/task.service.spec.ts`

```typescript
describe('filterByPriority', () => {
  it('should return only tasks with the specified priority', () => {
    service.addTask('High 1', 'Desc', 'high');
    service.addTask('Medium 1', 'Desc', 'medium');
    service.addTask('High 2', 'Desc', 'high');

    const highTasks = service.filterByPriority('high');

    expect(highTasks.length).toBe(2);
    expect(highTasks.every(t => t.priority === 'high')).toBeTrue();
  });

  it('should return empty array when no tasks match priority', () => {
    service.addTask('Medium', 'Desc', 'medium');

    const highTasks = service.filterByPriority('high');

    expect(highTasks.length).toBe(0);
  });
});
```

---

## Exercise 4: Tests for `searchTasks`
**File:** `src/app/services/task.service.spec.ts`

```typescript
describe('searchTasks', () => {
  it('should find tasks by partial title match', () => {
    service.addTask('Build feature', 'Desc');
    service.addTask('Fix bug', 'Desc');
    service.addTask('Build another', 'Desc');

    const results = service.searchTasks('Build');

    expect(results.length).toBe(2);
  });

  it('should be case-insensitive', () => {
    service.addTask('UPPERCASE TASK', 'Desc');

    const results = service.searchTasks('uppercase');

    expect(results.length).toBe(1);
    expect(results[0].title).toBe('UPPERCASE TASK');
  });

  it('should return empty array when no matches found', () => {
    service.addTask('Some task', 'Desc');

    const results = service.searchTasks('nonexistent');

    expect(results.length).toBe(0);
  });
});
```

---

## Exercise 5: Test `openEditTaskForm`
**File:** `src/app/components/kanban-board/kanban-board.component.spec.ts`

```typescript
it('should set editingTask when opening edit form', () => {
  const mockTask: Task = {
    id: '123',
    title: 'Test Task',
    description: 'Description',
    status: 'todo',
    priority: 'medium',
    createdAt: new Date(),
    updatedAt: new Date()
  };

  component.openEditTaskForm(mockTask);

  expect(component.showTaskForm()).toBeTrue();
  expect(component.editingTask()).toEqual(mockTask);
});
```

---

## Exercise 6: Test `closeTaskForm`
**File:** `src/app/components/kanban-board/kanban-board.component.spec.ts`

```typescript
it('should reset state when closing task form', () => {
  const mockTask: Task = {
    id: '123',
    title: 'Test Task',
    description: 'Description',
    status: 'todo',
    priority: 'medium',
    createdAt: new Date(),
    updatedAt: new Date()
  };

  // First open the form
  component.openEditTaskForm(mockTask);
  expect(component.showTaskForm()).toBeTrue();

  // Then close it
  component.closeTaskForm();

  expect(component.showTaskForm()).toBeFalse();
  expect(component.editingTask()).toBeNull();
});
```

---

## Exercise 7: Test drag and drop
**File:** `src/app/components/task-column/task-column.component.spec.ts`

```typescript
describe('Drag and Drop', () => {
  it('should emit taskMove when task is dropped', () => {
    const mockDataTransfer = {
      getData: () => 'task-123',
      setData: () => {}
    };
    const mockEvent = {
      preventDefault: () => {},
      currentTarget: document.createElement('div'),
      dataTransfer: mockDataTransfer
    } as unknown as DragEvent;

    spyOn(component.taskMove, 'emit');

    component.onDrop(mockEvent);

    expect(component.taskMove.emit).toHaveBeenCalledWith({
      taskId: 'task-123',
      newStatus: 'todo'
    });
  });

  it('should add drag-over class on dragover', () => {
    const element = document.createElement('div');
    const mockEvent = {
      preventDefault: () => {},
      currentTarget: element
    } as unknown as DragEvent;

    component.onDragOver(mockEvent);

    expect(element.classList.contains('drag-over')).toBeTrue();
  });
});
```

---

## Exercise 8: Implement `getPriorityClass`
**File:** `src/app/components/task-card/task-card.component.ts`

```typescript
getPriorityClass(): string {
  return `priority-${this.task().priority}`;
}
```

---

## Exercise 9: Implement `isStale`
**File:** `src/app/components/task-card/task-card.component.ts`

```typescript
isStale(): boolean {
  const sevenDaysMs = 7 * 24 * 60 * 60 * 1000;
  const taskDate = this.task().updatedAt.getTime();
  return Date.now() - taskDate > sevenDaysMs;
}
```

---

## Exercise 10: Tests for `getPriorityClass`
**File:** `src/app/components/task-card/task-card.component.spec.ts`

```typescript
describe('getPriorityClass', () => {
  it('should return "priority-high" for high priority tasks', () => {
    fixture.componentRef.setInput('task', createMockTask({ priority: 'high' }));
    fixture.detectChanges();

    expect(component.getPriorityClass()).toBe('priority-high');
  });

  it('should return "priority-medium" for medium priority tasks', () => {
    fixture.componentRef.setInput('task', createMockTask({ priority: 'medium' }));
    fixture.detectChanges();

    expect(component.getPriorityClass()).toBe('priority-medium');
  });

  it('should return "priority-low" for low priority tasks', () => {
    fixture.componentRef.setInput('task', createMockTask({ priority: 'low' }));
    fixture.detectChanges();

    expect(component.getPriorityClass()).toBe('priority-low');
  });
});
```

---

## Exercise 11: Tests for `isStale`
**File:** `src/app/components/task-card/task-card.component.spec.ts`

```typescript
describe('isStale', () => {
  it('should return true for tasks updated more than 7 days ago', () => {
    const eightDaysAgo = new Date(Date.now() - 8 * 24 * 60 * 60 * 1000);
    fixture.componentRef.setInput('task', createMockTask({ updatedAt: eightDaysAgo }));
    fixture.detectChanges();

    expect(component.isStale()).toBeTrue();
  });

  it('should return false for recently updated tasks', () => {
    const yesterday = new Date(Date.now() - 24 * 60 * 60 * 1000);
    fixture.componentRef.setInput('task', createMockTask({ updatedAt: yesterday }));
    fixture.detectChanges();

    expect(component.isStale()).toBeFalse();
  });
});
```

---

## Exercise 12: Custom validator
**File:** `src/app/components/task-form/task-form.component.ts`

```typescript
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Add this function to the file
function forbiddenTitleValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const forbidden = control.value?.toLowerCase().includes('test');
    return forbidden ? { forbiddenTitle: true } : null;
  };
}

// Update the form initialization in ngOnInit:
this.form = this.fb.group({
  title: [
    this.task()?.title ?? '', 
    [Validators.required, Validators.minLength(3), forbiddenTitleValidator()]
  ],
  description: [this.task()?.description ?? ''],
  priority: [this.task()?.priority ?? 'medium', Validators.required]
});
```

---

## Exercise 13: Test cancel functionality
**File:** `src/app/components/task-form/task-form.component.spec.ts`

```typescript
describe('Cancel', () => {
  beforeEach(() => {
    fixture.componentRef.setInput('task', null);
    fixture.detectChanges();
  });

  it('should emit formClose when cancel button is clicked', () => {
    spyOn(component.formClose, 'emit');

    const cancelButton = fixture.nativeElement.querySelector('.btn-secondary');
    cancelButton.click();

    expect(component.formClose.emit).toHaveBeenCalled();
  });

  it('should emit formClose when overlay is clicked', () => {
    spyOn(component.formClose, 'emit');

    const overlay = fixture.nativeElement.querySelector('.modal-overlay');
    overlay.click();

    expect(component.formClose.emit).toHaveBeenCalled();
  });
});
```

---

## Exercise 14: Async form submission test
**File:** `src/app/components/task-form/task-form.component.spec.ts`

```typescript
describe('Async Submission', () => {
  beforeEach(() => {
    fixture.componentRef.setInput('task', null);
    fixture.detectChanges();
  });

  it('should set isSubmitting during submission', fakeAsync(() => {
    component.form.patchValue({
      title: 'Valid Title',
      description: 'Description',
      priority: 'high'
    });

    expect(component.isSubmitting()).toBeFalse();

    component.onSubmit();
    tick();

    // After submission completes, isSubmitting should be false again
    expect(component.isSubmitting()).toBeFalse();
  }));
});
```

---

## Exercise 15: E2E - Drag and drop
**File:** `e2e/example.spec.ts`

```typescript
test('should move task via drag and drop', async ({ page }) => {
  await page.goto('/');

  // Create a task
  await page.click('.btn-add');
  await page.fill('#title', 'Drag Me');
  await page.click('button[type="submit"]');

  // Get the task card and target column
  const taskCard = page.locator('.task-card').first();
  const inProgressColumn = page.locator('app-task-column').nth(1);

  // Drag and drop
  await taskCard.dragTo(inProgressColumn);

  // Verify task is now in "In Progress" column
  const inProgressTasks = inProgressColumn.locator('.task-card');
  await expect(inProgressTasks).toHaveCount(1);
  await expect(inProgressTasks.first()).toContainText('Drag Me');
});
```

---

## Exercise 16: E2E - Form validation
**File:** `e2e/example.spec.ts`

```typescript
test('should show validation error for empty title', async ({ page }) => {
  await page.goto('/');

  // Open form
  await page.click('.btn-add');

  // Try to submit without title
  await page.click('button[type="submit"]');

  // Check for error message
  await expect(page.locator('.error')).toContainText('Title is required');
});
```

---

## Exercise 17: E2E - Task count updates
**File:** `e2e/example.spec.ts`

```typescript
test('should update task counts in header', async ({ page }) => {
  await page.goto('/');

  // Check initial counts
  await expect(page.locator('.stat').first()).toContainText('Total: 0');

  // Create first task
  await page.click('.btn-add');
  await page.fill('#title', 'Task 1');
  await page.click('button[type="submit"]');

  // Check counts updated
  await expect(page.locator('.stat').first()).toContainText('Total: 1');
  await expect(page.locator('.stat.todo')).toContainText('To Do: 1');

  // Create second task
  await page.click('.btn-add');
  await page.fill('#title', 'Task 2');
  await page.click('button[type="submit"]');

  // Verify final counts
  await expect(page.locator('.stat').first()).toContainText('Total: 2');
  await expect(page.locator('.stat.todo')).toContainText('To Do: 2');
});
```

---

## Exercise 18: E2E - Persistence
**File:** `e2e/example.spec.ts`

```typescript
test('should persist tasks after page reload', async ({ page }) => {
  await page.goto('/');

  // Create a task
  await page.click('.btn-add');
  await page.fill('#title', 'Persistent Task');
  await page.fill('#description', 'Should survive reload');
  await page.click('button[type="submit"]');

  // Verify task exists
  await expect(page.getByText('Persistent Task')).toBeVisible();

  // Reload page (don't clear localStorage this time)
  await page.reload();

  // Verify task still exists
  await expect(page.getByText('Persistent Task')).toBeVisible();
  await expect(page.getByText('Should survive reload')).toBeVisible();
});
```

---

## Common Mistakes to Watch For

1. **Not using `fixture.detectChanges()`** after setting inputs
2. **Forgetting `async/await`** in E2E tests
3. **Not mocking dependencies** properly in unit tests
4. **Testing implementation instead of behavior**
5. **Not cleaning up state** between tests

## Grading Rubric

| Exercise | Points | Criteria |
|----------|--------|----------|
| 1-2 | 5 each | Implementation works correctly |
| 3-4 | 5 each | Tests cover edge cases |
| 5-7 | 5 each | Integration tests use proper mocking |
| 8-9 | 5 each | Implementation is clean and efficient |
| 10-14 | 5 each | Tests are isolated and meaningful |
| 15-18 | 10 each | E2E tests are reliable and complete |

**Total: 100 points**
