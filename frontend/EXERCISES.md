# Angular Testing Course - Exercises

This Kanban Board application contains **18 exercises** covering unit, integration, and E2E testing in Angular 20.

## Prerequisites

- Node.js 20+
- Angular CLI 20+
- Basic understanding of Angular signals, components, and services

## Getting Started

```bash
# Install dependencies
npm install

# Run the application
npm start

# Run unit/integration tests
npm test

# Run E2E tests
npx playwright test
```

---

## Unit Testing Exercises (TaskService)

### Exercise 1: Implement `filterByPriority` method
**File:** `src/app/services/task.service.ts`

Implement a method that filters tasks by priority level.

```typescript
filterByPriority(priority: TaskPriority): Task[]
```

**Requirements:**
- Return all tasks matching the given priority ('low', 'medium', 'high')
- Use the tasks signal to get current tasks

**Hints:**
- Use `this.tasks()` to access current task list
- Use `Array.filter()` to filter by priority

---

### Exercise 2: Implement `searchTasks` method
**File:** `src/app/services/task.service.ts`

Implement a method that searches tasks by title.

```typescript
searchTasks(query: string): Task[]
```

**Requirements:**
- Return tasks where title contains the query string
- Search should be case-insensitive
- Return empty array if no matches

**Hints:**
- Use `toLowerCase()` for case-insensitive comparison
- Use `includes()` for partial matching

---

### Exercise 3: Write tests for `filterByPriority`
**File:** `src/app/services/task.service.spec.ts`

Write unit tests for the `filterByPriority` method.

**Test cases to implement:**
1. Should return only tasks with the specified priority
2. Should return empty array when no tasks match priority

---

### Exercise 4: Write tests for `searchTasks`
**File:** `src/app/services/task.service.spec.ts`

Write unit tests for the `searchTasks` method.

**Test cases to implement:**
1. Should find tasks by partial title match
2. Should be case-insensitive
3. Should return empty array when no matches found

---

## Integration Testing Exercises (Components)

### Exercise 5: Test `openEditTaskForm` in KanbanBoardComponent
**File:** `src/app/components/kanban-board/kanban-board.component.spec.ts`

Test that when `openEditTaskForm` is called with a task, both `showTaskForm` and `editingTask` signals are set correctly.

**Requirements:**
- Verify `showTaskForm()` returns `true`
- Verify `editingTask()` returns the passed task

---

### Exercise 6: Test `closeTaskForm` in KanbanBoardComponent
**File:** `src/app/components/kanban-board/kanban-board.component.spec.ts`

Test that `closeTaskForm` resets both signals.

**Requirements:**
- First set some state (call `openEditTaskForm`)
- Then call `closeTaskForm`
- Verify both signals are reset

---

### Exercise 7: Test drag and drop in TaskColumnComponent
**File:** `src/app/components/task-column/task-column.component.spec.ts`

Test the drag and drop functionality.

**Test cases:**
1. Should emit `taskMove` when task is dropped
2. Should add `drag-over` class on dragover event

**Hints:**
- Create a mock `DragEvent` with `dataTransfer`
- Use `spyOn()` to verify output emissions

---

### Exercise 8: Implement `getPriorityClass` in TaskCardComponent
**File:** `src/app/components/task-card/task-card.component.ts`

Implement a method that returns a CSS class based on task priority.

**Requirements:**
- Return `'priority-high'` for high priority
- Return `'priority-medium'` for medium priority
- Return `'priority-low'` for low priority

---

### Exercise 9: Implement `isStale` in TaskCardComponent
**File:** `src/app/components/task-card/task-card.component.ts`

Implement a method that returns true if the task was updated more than 7 days ago.

**Requirements:**
- Compare `task().updatedAt` with current date
- Return `true` if more than 7 days old
- Return `false` otherwise

**Hints:**
- Use `Date.now()` and `getTime()` for comparison
- 7 days = 7 * 24 * 60 * 60 * 1000 milliseconds

---

### Exercise 10: Write tests for `getPriorityClass`
**File:** `src/app/components/task-card/task-card.component.spec.ts`

Write unit tests for the `getPriorityClass` method.

**Test cases:**
1. Should return `"priority-high"` for high priority tasks
2. Should return `"priority-medium"` for medium priority tasks
3. Should return `"priority-low"` for low priority tasks

---

### Exercise 11: Write tests for `isStale`
**File:** `src/app/components/task-card/task-card.component.spec.ts`

Write unit tests for the `isStale` method.

**Test cases:**
1. Should return `true` for tasks updated more than 7 days ago
2. Should return `false` for recently updated tasks

**Hints:**
- Create mock tasks with different `updatedAt` dates
- Use `new Date(Date.now() - 8 * 24 * 60 * 60 * 1000)` for 8 days ago

---

### Exercise 12: Implement custom validator (Bonus)
**File:** `src/app/components/task-form/task-form.component.ts`

Create a custom validator that prevents titles containing "test" (case-insensitive).

**Requirements:**
- Create a validator function
- Return `{ forbiddenTitle: true }` if invalid
- Return `null` if valid
- Add to the title form control

---

### Exercise 13: Test cancel functionality
**File:** `src/app/components/task-form/task-form.component.spec.ts`

Write tests for cancel functionality.

**Test cases:**
1. Should emit `formClose` when cancel button is clicked
2. Should emit `formClose` when overlay is clicked

---

### Exercise 14: Write async form submission test
**File:** `src/app/components/task-form/task-form.component.spec.ts`

Write an async test using `fakeAsync` and `tick`.

**Test case:**
- Should set `isSubmitting` during submission

---

## E2E Testing Exercises (Playwright)

### Exercise 15: Test drag and drop
**File:** `e2e/example.spec.ts`

Write an E2E test for drag and drop functionality.

**Requirements:**
1. Create a task
2. Drag from "To Do" to "In Progress"
3. Verify task appears in new column

**Hints:**
- Use `page.dragAndDrop(source, target)`

---

### Exercise 16: Test form validation
**File:** `e2e/example.spec.ts`

Write an E2E test for form validation.

**Requirements:**
1. Open task form
2. Try to submit without title
3. Verify error message appears

---

### Exercise 17: Test task count updates
**File:** `e2e/example.spec.ts`

Write an E2E test for task count updates.

**Requirements:**
1. Check initial counts (all zeros)
2. Create multiple tasks
3. Verify counts update in header

---

### Exercise 18: Test persistence
**File:** `e2e/example.spec.ts`

Write an E2E test for localStorage persistence.

**Requirements:**
1. Create a task
2. Reload the page
3. Verify task still exists

---

## Running Tests

```bash
# Run all unit/integration tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run specific test file
npm test -- --include=**/task.service.spec.ts

# Run E2E tests
npx playwright test

# Run E2E tests with UI
npx playwright test --ui

# Run E2E tests headed (see browser)
npx playwright test --headed
```

## Tips for Success

1. **Start with the implementation** - Implement the methods (Exercises 1, 2, 8, 9) before writing their tests
2. **Use the existing tests as reference** - Look at the working tests for patterns
3. **Test one thing at a time** - Each test should verify a single behavior
4. **Use meaningful test names** - Describe what the test verifies
5. **Clean state between tests** - Use `beforeEach` to reset state

Good luck! 🚀
