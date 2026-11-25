import { test, expect } from '@playwright/test';
import { createGoal, createGroup, newUserSession, register, uniq } from '../helpers/app';

test.describe('Search', () => {
  test('searching finds goals, people and groups by name', async ({ page, browser }) => {
    const key = `zq${uniq()}`;
    const other = await newUserSession(browser, key); // user name contains the key
    const goal = await createGoal(other.page, { name: `${key} goal` });
    const group = await createGroup(other.page, `${key} group`);

    await register(page, 'sr');
    await page.locator('input[name="searchText"]').fill(key);
    await page.locator('input[name="searchText"]').press('Enter');

    await expect(page).toHaveURL(/\/Search\/SearchAll/); // the header search form posts to this URL
    await expect(page.locator('.well').first()).toContainText(`Search results for "${key}" are..`);
    await expect(page.locator('#goallistcontainer').getByRole('link', { name: goal.name })).toBeVisible();
    await expect(
      page.locator('#peoplelistcontainer').getByRole('link', { name: other.user.userName }).first(),
    ).toBeVisible();
    await expect(page.locator('#grouplistcontainer').getByRole('link', { name: group.name })).toBeVisible();
    await expect(page.locator('#grouplistcontainer')).toContainText('1 Members');
    await other.context.close();
  });

  test('result links lead to the goal, the profile and the group', async ({ page, browser }) => {
    const key = `zq${uniq()}`;
    const other = await newUserSession(browser, key);
    const goal = await createGoal(other.page, { name: `${key} goal` });
    const group = await createGroup(other.page, `${key} group`);

    await register(page, 'sr');
    const search = `/Search/SearchAll?searchText=${key}`;
    await page.goto(search);
    await page.locator('#goallistcontainer').getByRole('link', { name: goal.name }).click();
    await expect(page).toHaveURL(new RegExp(`/Goal/Index/${goal.id}$`));

    await page.goto(search);
    await page.locator('#grouplistcontainer').getByRole('link', { name: group.name }).click();
    await expect(page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));

    await page.goto(search);
    await page.locator('#peoplelistcontainer').getByRole('link', { name: other.user.userName }).first().click();
    await expect(page).toHaveURL(new RegExp(`/Account/UserProfile/${other.user.id}$`));
    await other.context.close();
  });

  test('a search with no matches shows empty-state messages in all three sections', async ({ page }) => {
    await register(page, 'sr');
    const none = `nomatch${uniq()}`;
    await page.goto(`/Search/SearchAll?searchText=${none}`);
    await expect(page.getByText('No goals to display').first()).toBeVisible();
    await expect(page.getByText('No people to display')).toBeVisible();
    await expect(page.getByText('No group to display')).toBeVisible();
  });

  test("search results do not include other users' private goals", async ({ page, browser }) => {
    const key = `zq${uniq()}`;
    const owner = await newUserSession(browser, 'sp');
    const priv = await createGoal(owner.page, { name: `${key} private`, isPrivate: true });
    const pub = await createGoal(owner.page, { name: `${key} public` });

    await register(page, 'sr');
    await page.goto(`/Search/SearchAll?searchText=${key}`);
    await expect(page.locator('#goallistcontainer').getByRole('link', { name: pub.name })).toBeVisible();
    await expect(page.locator('#goallistcontainer').getByRole('link', { name: priv.name })).toHaveCount(0);
    await owner.context.close();
  });
});
