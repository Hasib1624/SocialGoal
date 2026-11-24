import { test, expect } from '@playwright/test';
import { register, login, logout, uniq, PASSWORD } from '../helpers/app';

test.describe('Registration', () => {
  test('a new visitor can register and lands signed in on the home page', async ({ page }) => {
    const userName = `reg${uniq()}`;
    await page.goto('/Account/Register');
    await expect(page.getByRole('heading', { name: 'Register.', exact: true })).toBeVisible();
    await page.locator('#UserName').fill(userName);
    await page.locator('#Password').fill(PASSWORD);
    await page.locator('#ConfirmPassword').fill(PASSWORD);
    await page.getByRole('button', { name: 'Register' }).click();

    await expect(page).toHaveURL(/\/$/);
    await expect(page.locator('a[href="#"]', { hasText: userName })).toBeAttached();
    await expect(page.locator('a[href="/Account/LogOff"]')).toBeAttached();
  });

  test('registering with a user name that already exists shows an error', async ({ page, browser }) => {
    const first = await browser.newContext();
    const user = await register(await first.newPage(), 'dup');
    await first.close();

    await page.goto('/Account/Register');
    await page.locator('#UserName').fill(user.userName);
    await page.locator('#Password').fill(PASSWORD);
    await page.locator('#ConfirmPassword').fill(PASSWORD);
    await page.getByRole('button', { name: 'Register' }).click();

    await expect(page).toHaveURL(/\/Account\/Register/);
    await expect(page.locator('.validation-summary-errors')).toContainText(`Name ${user.userName} is already taken.`);
  });

  test('registration with empty fields shows required-field errors', async ({ page }) => {
    await page.goto('/Account/Register');
    await page.getByRole('button', { name: 'Register' }).click();
    await expect(page.locator('.validation-summary-errors')).toContainText('The User name field is required.');
    await expect(page.locator('.validation-summary-errors')).toContainText('The Password field is required.');
  });

  test('registration rejects a password shorter than 6 characters', async ({ page }) => {
    await page.goto('/Account/Register');
    await page.locator('#UserName').fill(`short${uniq()}`);
    await page.locator('#Password').fill('abc');
    await page.locator('#ConfirmPassword').fill('abc');
    await page.getByRole('button', { name: 'Register' }).click();
    await expect(page.locator('.validation-summary-errors')).toContainText(
      'The Password must be at least 6 characters long.',
    );
  });

  test('registration rejects mismatching password confirmation', async ({ page }) => {
    await page.goto('/Account/Register');
    await page.locator('#UserName').fill(`mism${uniq()}`);
    await page.locator('#Password').fill(PASSWORD);
    await page.locator('#ConfirmPassword').fill('Different1!');
    await page.getByRole('button', { name: 'Register' }).click();
    await expect(page.locator('.validation-summary-errors')).toContainText(
      "'Confirm password' and 'Password' do not match.",
    );
  });
});

test.describe('Login and logout', () => {
  test('a registered user can sign out and sign in again', async ({ page }) => {
    const user = await register(page, 'li');
    await logout(page);

    await login(page, user.userName);
    await expect(page).toHaveURL(/\/$/);
    await expect(page.locator('a[href="#"]', { hasText: user.userName })).toBeAttached();
  });

  test('sign out ends the session: protected pages redirect to login afterwards', async ({ page }) => {
    const user = await register(page, 'lo');
    await page.locator('a[href="#"]', { hasText: user.userName }).click(); // opens the user menu
    await page.locator('a[href="/Account/LogOff"]').click();
    await page.goto('/Goal/MyGoal');
    await expect(page).toHaveURL(/\/Account\/Login\?ReturnUrl=%2[fF]Goal%2[fF]MyGoal/);
  });

  test('login with a wrong password shows an error and stays on the login page', async ({ page }) => {
    const user = await register(page, 'bad');
    await logout(page);
    await login(page, user.userName, 'WrongPassw0rd!');
    await expect(page).toHaveURL(/\/Account\/Login/);
    await expect(page.locator('.validation-summary-errors')).toContainText('Invalid username or password.');
  });

  test('login with an unknown user name shows an error', async ({ page }) => {
    await login(page, `nobody${uniq()}`);
    await expect(page.locator('.validation-summary-errors')).toContainText('Invalid username or password.');
  });

  test('login with empty fields shows required-field validation', async ({ page }) => {
    await page.goto('/Account/Login');
    await page.getByRole('button', { name: 'Log in' }).click();
    await expect(page.locator('#loginForm')).toContainText('The User name field is required.');
    await expect(page.locator('#loginForm')).toContainText('The Password field is required.');
  });

  test('login honours the ReturnUrl after redirect from a protected page', async ({ page, browser }) => {
    const ctx = await browser.newContext();
    const user = await register(await ctx.newPage(), 'ret');
    await ctx.close();

    await page.goto('/Goal/MyGoal');
    await expect(page).toHaveURL(/ReturnUrl/);
    await page.locator('#Email').fill(user.userName);
    await page.locator('#Password').fill(user.password);
    await page.getByRole('button', { name: 'Log in' }).click();
    await expect(page).toHaveURL(/\/Goal\/MyGoal$/);
  });

  test('login page renders the external login button and a Register link', async ({ page }) => {
    await page.goto('/Account/Login');
    await expect(page.locator('#socialLoginForm')).toContainText('Use another service to log in.');
    // current behavior: only Google is configured (Facebook/Microsoft/Twitter are not enabled)
    await expect(page.locator('#socialLoginForm button[name="provider"]', { hasText: 'Google' })).toBeVisible();
    await page.getByRole('link', { name: 'Register' }).first().click();
    await expect(page).toHaveURL(/\/Account\/Register/);
  });
});
