import { test, expect } from '@playwright/test';

const protectedPaths = [
  '/Goal/Create',
  '/Goal/MyGoal',
  '/Goal/GoalList',
  '/Goal/Index/1',
  '/Group/CreateGroup',
  '/Group/GroupList',
  '/Group/Index/1',
  '/Notification/Index',
  '/Account/Followers',
  '/Account/Followings',
  '/Account/EditBasicInfo',
  '/Account/ImageUpload',
  '/Home/About',
];

test.describe('Anonymous access control', () => {
  test('the home page redirects an anonymous visitor to the login page', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveURL(/\/Account\/Login\?ReturnUrl=%2[fF]/);
    await expect(page.getByRole('heading', { name: 'Log in.', exact: true })).toBeVisible();
  });

  for (const path of protectedPaths) {
    test(`anonymous request to ${path} is redirected to login with a ReturnUrl`, async ({ page }) => {
      await page.goto(path);
      await expect(page).toHaveURL(/\/Account\/Login\?ReturnUrl=/);
      await expect(page.locator('#loginForm')).toBeVisible();
    });
  }

  test('Login and Register pages are reachable anonymously', async ({ page }) => {
    await page.goto('/Account/Login');
    await expect(page).toHaveURL(/\/Account\/Login$/);
    await page.goto('/Account/Register');
    await expect(page).toHaveURL(/\/Account\/Register$/);
  });
});
