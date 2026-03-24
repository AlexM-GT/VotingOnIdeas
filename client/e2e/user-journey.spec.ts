import { expect, test } from '@playwright/test';

const API_BASE = process.env.PW_API_BASE_URL ?? 'http://localhost:8080/api';

function uniqueUser() {
  const stamp = `${Date.now()}-${Math.floor(Math.random() * 100000)}`;
  return {
    username: `e2e_${stamp}`,
    email: `e2e_${stamp}@example.com`,
    password: 'Admin123!',
    ideaTitle: `E2E Idea ${stamp}`,
  };
}

async function openAccountMenu(page: import('@playwright/test').Page) {
  await page.getByRole('button', { name: 'Account menu' }).click();
}

test.beforeEach(async ({ request }) => {
  let backendAvailable = false;
  try {
    const response = await request.get(`${API_BASE}/ideas?page=1&pageSize=1`);
    backendAvailable = response.ok();
  } catch {
    backendAvailable = false;
  }

  test.skip(!backendAvailable, `Backend API is not available at ${API_BASE}`);
});

test('critical user journey: register, create/edit/rate/delete idea, logout', async ({ page }) => {
  const user = uniqueUser();

  await page.goto('/');

  await openAccountMenu(page);
  await page.getByRole('button', { name: 'Register' }).click();

  await page.getByLabel('Username').fill(user.username);
  await page.getByLabel('Email').fill(user.email);
  await page.getByLabel('Password').fill(user.password);
  await page.getByRole('button', { name: 'Register' }).click();

  await expect(page.getByText(`Hello, ${user.username}!`)).toBeVisible();
  await expect(page.getByText('Ideas')).toBeVisible();

  await page.getByRole('button', { name: 'Add idea' }).click();
  await expect(page.getByRole('heading', { name: 'New idea' })).toBeVisible();

  await page.getByLabel('Title').fill(user.ideaTitle);
  await page.getByLabel('Description').fill('This is an end-to-end test idea description.');
  await page.getByRole('button', { name: 'Create idea' }).click();

  await expect(page.getByRole('heading', { name: user.ideaTitle })).toBeVisible();

  await page.getByRole('button', { name: 'Edit' }).click();
  await expect(page.getByRole('heading', { name: 'Edit idea' })).toBeVisible();

  const editedTitle = `${user.ideaTitle} (edited)`;
  await page.getByLabel('Title').fill(editedTitle);
  await page.getByRole('button', { name: 'Save changes' }).click();

  await expect(page.getByRole('heading', { name: editedTitle })).toBeVisible();

  await page.getByRole('button', { name: '5 stars' }).click();
  await expect(page.getByText(/Average:\s*5\.0/)).toBeVisible();

  page.once('dialog', (dialog) => dialog.accept());
  await page.getByRole('button', { name: 'Delete' }).click();

  await expect(page.getByText('Ideas')).toBeVisible();
  await expect(page.getByText(editedTitle)).toHaveCount(0);

  await openAccountMenu(page);
  await page.getByRole('button', { name: 'Log out' }).click();

  await expect(page).toHaveURL(/\/login$/);
  await expect(page.getByRole('heading', { name: 'Sign in' })).toBeVisible();
});
