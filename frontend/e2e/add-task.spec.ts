import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:4200/');
  await page.getByRole('button', { name: '+ Add Task' }).click();
  await page.getByRole('textbox', { name: 'Title *' }).click();
  await page.getByRole('textbox', { name: 'Title *' }).fill('abc');
  await page.getByRole('textbox', { name: 'Description' }).click();
  await page.getByRole('textbox', { name: 'Description' }).fill('abc');
  await page.getByLabel('Priority *').selectOption('low');
  await page.getByRole('button', { name: 'Create' }).click();
});