import { test, expect } from '@playwright/test';

test.describe('Kanban Board E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Clear localStorage before each test
    await page.goto('/');
    await page.evaluate(() => localStorage.clear());
    await page.reload();
  });

  test('should display empty board with three columns', async ({ page }) => {
    await page.goto('/');

    // Check header
    await expect(page.locator('h1')).toContainText('Kanban Board');

    // Check three columns exist
    const columns = page.locator('app-task-column');
    await expect(columns).toHaveCount(3);

    // Check column titles
    await expect(page.getByText('To Do')).toBeVisible();
    await expect(page.getByText('In Progress')).toBeVisible();
    await expect(page.getByText('Done')).toBeVisible();
  });

  test('should create a new task', async ({ page }) => {
    await page.goto('/');

    // Click add task button
    await page.click('.btn-add');

    // Fill form
    await page.fill('#title', 'My New Task');
    await page.fill('#description', 'Task description');
    await page.selectOption('#priority', 'high');

    // Submit
    await page.click('button[type="submit"]');

    // Verify task appears in To Do column
    await expect(page.locator('.task-card')).toHaveCount(1);
    await expect(page.getByText('My New Task')).toBeVisible();
  });

  test('should edit an existing task', async ({ page }) => {
    await page.goto('/');

    // Create a task first
    await page.click('.btn-add');
    await page.fill('#title', 'Original Title');
    await page.click('button[type="submit"]');

    // Click edit button
    await page.click('.task-card .btn-icon[title="Edit"]');

    // Update title
    await page.fill('#title', 'Updated Title');
    await page.click('button[type="submit"]');

    // Verify updated title
    await expect(page.getByText('Updated Title')).toBeVisible();
    await expect(page.getByText('Original Title')).not.toBeVisible();
  });

  test('should delete a task', async ({ page }) => {
    await page.goto('/');

    // Create a task
    await page.click('.btn-add');
    await page.fill('#title', 'Task to Delete');
    await page.click('button[type="submit"]');

    // Accept the confirmation dialog
    page.on('dialog', dialog => dialog.accept());

    // Click delete button
    await page.click('.task-card .btn-icon[title="Delete"]');

    // Verify task is removed
    await expect(page.locator('.task-card')).toHaveCount(0);
  });

  // EXERCISE 15: Write E2E test for drag and drop
  // Test that a task can be dragged from "To Do" to "In Progress"
  test('should move task via drag and drop', async ({ page }) => {
    await page.goto('/');

    // Create a task first
    await page.click('.btn-add');
    await page.fill('#title', 'Drag Me Task');
    await page.click('button[type="submit"]');

    // Wait for task to appear in To Do column
    await expect(page.locator('app-task-column').first().getByText('Drag Me Task')).toBeVisible();

    // Get the task card and the "In Progress" column
    const taskCard = page.locator('.task-card').filter({ hasText: 'Drag Me Task' });
    const inProgressColumn = page.locator('app-task-column').nth(1).locator('.column');

    // Perform drag and drop
    await taskCard.dragTo(inProgressColumn);

    // Verify the task appears in the In Progress column
    await expect(page.locator('app-task-column').nth(1).getByText('Drag Me Task')).toBeVisible();

    // Verify the task is no longer in the To Do column
    await expect(page.locator('app-task-column').first().getByText('Drag Me Task')).not.toBeVisible();
  });

  // EXERCISE 16: Write E2E test for form validation
  // Test that submitting with empty title shows error message
  test('should show validation error for empty title', async ({ page }) => {
    await page.goto('/');

    // Open the task form
    await page.click('.btn-add');

    // Try to submit without entering title
    await page.click('button[type="submit"]');

    // Verify error message is displayed
    await expect(page.locator('.error')).toBeVisible();
    await expect(page.locator('.error')).toContainText('Title is required');
  });

  // EXERCISE 17: Write E2E test for task count updates
  // Test that header stats update correctly when tasks are created/moved
  test('should update task counts in header', async ({ page }) => {
    await page.goto('/');

    // Check initial counts (all zeros)
    await expect(page.getByText('Total: 0')).toBeVisible();
    await expect(page.getByText('To Do: 0')).toBeVisible();
    await expect(page.getByText('In Progress: 0')).toBeVisible();
    await expect(page.getByText('Done: 0')).toBeVisible();

    // Create first task
    await page.click('.btn-add');
    await page.fill('#title', 'Task One');
    await page.click('button[type="submit"]');

    // Verify counts update after first task
    await expect(page.getByText('Total: 1')).toBeVisible();
    await expect(page.getByText('To Do: 1')).toBeVisible();

    // Create second task
    await page.click('.btn-add');
    await page.fill('#title', 'Task Two');
    await page.click('button[type="submit"]');

    // Verify counts update after second task
    await expect(page.getByText('Total: 2')).toBeVisible();
    await expect(page.getByText('To Do: 2')).toBeVisible();

    // Drag first task to In Progress
    const taskCard = page.locator('.task-card').filter({ hasText: 'Task One' });
    const inProgressColumn = page.locator('app-task-column').nth(1).locator('.column');
    await taskCard.dragTo(inProgressColumn);

    // Verify counts update after move
    await expect(page.getByText('Total: 2')).toBeVisible();
    await expect(page.getByText('To Do: 1')).toBeVisible();
    await expect(page.getByText('In Progress: 1')).toBeVisible();
  });

  // EXERCISE 18: Write E2E test for persistence
  // Test that tasks persist after page reload (backend API)
  test('should persist tasks after page reload', async ({ page }) => {
    await page.goto('/');

    // Create a task
    await page.click('.btn-add');
    await page.fill('#title', 'Persistent Task');
    await page.fill('#description', 'This should persist');
    await page.selectOption('#priority', 'high');
    await page.click('button[type="submit"]');

    // Verify task appears
    await expect(page.getByText('Persistent Task')).toBeVisible();
    await expect(page.getByText('Total: 1')).toBeVisible();

    // Reload the page
    await page.reload();

    // Verify task still exists after reload
    await expect(page.getByText('Persistent Task')).toBeVisible();
    await expect(page.getByText('Total: 1')).toBeVisible();
  });
});

