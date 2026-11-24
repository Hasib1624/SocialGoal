import { test, expect } from '@playwright/test';
import { register, uniq } from '../helpers/app';
import path from 'path';
import fs from 'fs';
import os from 'os';

test.describe('Profile', () => {
  test('the profile page shows the user name and the Basic Info / Personal Info sections', async ({ page }) => {
    const user = await register(page, 'pr');
    await page.goto(`/Account/UserProfile/${user.id}`);
    await expect(page.locator('body')).toContainText(new RegExp(`User Name:\\s+${user.userName}`));
    await expect(page.locator('body')).toContainText('Basic Info');
    await expect(page.locator('body')).toContainText('Personal Info');
    await expect(page.locator('body')).toContainText('Recent Activities');
    await expect(page.locator('#editprofile')).toBeVisible();
    await expect(page.locator('#edit')).toBeVisible();
  });

  test('the user can edit basic info (name, email, gender)', async ({ page }) => {
    const user = await register(page, 'pr');
    const id = uniq();
    await page.goto(`/Account/UserProfile/${user.id}`);
    await page.locator('#editprofile').click();
    await page.locator('#divBasicInfo #FirstName').fill(`First${id}`);
    await page.locator('#divBasicInfo #LastName').fill(`Last${id}`);
    await page.locator('#divBasicInfo #Email').fill(`${id}@example.com`);
    await page.locator('#divBasicInfo input[name="Gender"][value="True"]').check();
    await page.locator('#divBasicInfo').getByRole('button', { name: 'Save' }).click();

    await expect(page).toHaveURL(new RegExp(`/Account/UserProfile/${user.id}$`));
    await expect(page.locator('body')).toContainText(`First${id}`);
    await expect(page.locator('body')).toContainText(`Last${id}`);
    await expect(page.locator('body')).toContainText(`${id}@example.com`);
  });

  test('basic info requires first and last name', async ({ page }) => {
    const user = await register(page, 'pr');
    await page.goto(`/Account/UserProfile/${user.id}`);
    await page.locator('#editprofile').click();
    await page.locator('#divBasicInfo').getByRole('button', { name: 'Save' }).click();
    // current behavior: the POST re-renders the edit form as a bare partial with "*" markers
    await expect(page.locator('.field-validation-error').first()).toHaveText('*');
  });

  test('the user can edit personal info (address, city, state, country)', async ({ page }) => {
    const user = await register(page, 'pr');
    // personal info lives in the same UserProfile record, so first/last name must be valid when saving
    await page.goto(`/Account/UserProfile/${user.id}`);
    await page.locator('#editprofile').click();
    await page.locator('#divBasicInfo #FirstName').fill('Pat');
    await page.locator('#divBasicInfo #LastName').fill('Tester');
    await page.locator('#divBasicInfo').getByRole('button', { name: 'Save' }).click();
    await expect(page).toHaveURL(new RegExp(`/Account/UserProfile/${user.id}$`));

    await page.locator('#edit').click();
    await page.locator('#divPersonalInfo #Address').fill('1 Test Street');
    await page.locator('#divPersonalInfo #City').fill('Dhaka');
    await page.locator('#divPersonalInfo #State').fill('Dhaka');
    await page.locator('#divPersonalInfo #Country').fill('Bangladesh');
    await page.locator('#divPersonalInfo').getByRole('button', { name: 'Save' }).click();

    await expect(page).toHaveURL(new RegExp(`/Account/UserProfile/${user.id}$`));
    await expect(page.locator('body')).toContainText('1 Test Street');
    await expect(page.locator('body')).toContainText('Dhaka');
    await expect(page.locator('body')).toContainText('Bangladesh');
  });

  test('the user can upload a profile picture', async ({ page }) => {
    const user = await register(page, 'pr');
    await page.goto(`/Account/UserProfile/${user.id}`);
    await page.locator('#editpicture').click();

    // 8x8 red PNG
    const png = Buffer.from(
      'iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAIAAABLbSncAAAAEklEQVR4nGP8z8Dwn4EIwDiqEAAdSwH/8vN3owAAAABJRU5ErkJggg==',
      'base64',
    );
    const file = path.join(os.tmpdir(), `avatar-${uniq()}.png`);
    fs.writeFileSync(file, png);
    try {
      await page.locator('#profilepic input[type="file"]').setInputFiles(file);
      await page.locator('#profilepic input[type="submit"]').click();
      await expect(page).toHaveURL(new RegExp(`/Account/UserProfile/${user.id}$`));
      await expect(page.locator('img[src*="/Content/ProfilePics/"]').first()).toBeAttached();
    } finally {
      fs.unlinkSync(file);
    }
  });

  // current behavior (possible bug): the change-password page crashes (section "Scripts" not rendered by its layout)
  test('/Account/Manage (change password) currently fails with a server error', async ({ page }) => {
    await register(page, 'pr');
    const res = await page.goto('/Account/Manage');
    expect(res!.status()).toBe(500);
  });

  test('the About and Contact pages render for a signed-in user', async ({ page }) => {
    await register(page, 'pr');
    await page.goto('/Home/About');
    await expect(page.getByRole('heading', { name: 'About.' })).toBeVisible();
    await page.goto('/Home/Contact');
    await expect(page.getByRole('heading', { name: 'Contact.' })).toBeVisible();
  });
});
