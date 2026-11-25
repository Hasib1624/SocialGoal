import { test, expect } from '@playwright/test';
import { createGoal, newUserSession, inviteViaSearch, postUpdate, register, setBasicInfo, setDate, uniq } from '../helpers/app';

test.describe('Goals: create / view / edit / delete', () => {
  test('a user can create a goal and sees its details page', async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page, { desc: 'Run every day', metric: 'Km', target: '100' });

    await expect(page.locator('h4', { hasText: goal.name })).toBeVisible();
    await expect(page.locator('.well').first()).toContainText('Run every day');
    await expect(page.locator('.well').first()).toContainText('Start Date : 01 Jan 2026');
    await expect(page.locator('.well').first()).toContainText('End Date : 31 Dec 2030');
    await expect(page.locator('.well').first()).toContainText('Metric : Km');
    await expect(page.locator('.well').first()).toContainText('Target: 100');
    await expect(page.locator('.well').first()).toContainText('GoalStatus:In Progress');
    await expect(page.locator('#divupdate')).toContainText('No updates on this goal');
  });

  test('a goal without metric and target shows "No Metric" and "No Target"', async ({ page }) => {
    await register(page, 'g');
    await createGoal(page);
    await expect(page.locator('.well').first()).toContainText('No Metric');
    await expect(page.locator('.well').first()).toContainText('No Target');
  });

  test('creating a goal with required fields empty shows validation markers', async ({ page }) => {
    await register(page, 'g');
    await page.goto('/Goal/Create');
    await page.locator('#submitbtn').click();
    await expect(page).toHaveURL(/\/Goal\/Create/);
    await expect(page.locator('.field-validation-error').first()).toHaveText('*');
    expect(await page.locator('.field-validation-error').count()).toBeGreaterThanOrEqual(3);
  });

  // current behavior (possible bug): the client-side "Please enter valid status" message never shows;
  // the form is simply redisplayed (server-side binding fails) with an empty validation message.
  test('creating a goal with a non-numeric target is not accepted', async ({ page }) => {
    await register(page, 'g');
    await page.goto('/Goal/Create');
    await page.locator('#GoalName').fill(`Goal ${uniq()}`);
    await page.locator('#Desc').fill('d');
    await setDate(page, '#StartDate', '01-Jan-2026');
    await setDate(page, '#EndDate', '31-Dec-2030');
    await page.locator('#Target').fill('abc');
    await page.locator('#submitbtn').click();
    await expect(page).toHaveURL(/\/Goal\/Create/);
    await expect(page.locator('#NumberValidation')).toHaveText('');
  });

  test('end date before start date is rejected', async ({ page }) => {
    await register(page, 'g');
    await page.goto('/Goal/Create');
    await page.locator('#GoalName').fill(`Goal ${uniq()}`);
    await page.locator('#Desc').fill('d');
    await setDate(page, '#StartDate', '01-Jan-2030');
    await setDate(page, '#EndDate', '01-Jan-2026');
    await page.locator('#submitbtn').click();
    await expect(page).toHaveURL(/\/Goal\/Create/);
    await expect(page.locator('form#form')).toContainText('Enddate cannot be before Start date.');
  });

  test('goal names must be unique', async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);
    await page.goto('/Goal/Create');
    await page.locator('#GoalName').fill(goal.name);
    await page.locator('#Desc').fill('d');
    await setDate(page, '#StartDate', '01-Jan-2026');
    await setDate(page, '#EndDate', '31-Dec-2030');
    await page.locator('#submitbtn').click();
    await expect(page).toHaveURL(/\/Goal\/Create/);
    await expect(page.locator('form#form')).toContainText('Goal already exists.');
  });

  test('the owner can edit a goal', async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);
    await page.locator('#editBtn').click();
    await expect(page).toHaveURL(new RegExp(`/Goal/Edit/${goal.id}$`));

    const newName = `${goal.name} edited`;
    await page.locator('#GoalName').fill(newName);
    await page.locator('#Desc').fill('Edited description');
    await page.getByRole('button', { name: 'Save' }).click();

    await expect(page).toHaveURL(new RegExp(`/Goal/Index/${goal.id}$`));
    await expect(page.locator('h4', { hasText: newName })).toBeVisible();
    await expect(page.locator('.well').first()).toContainText('Edited description');
  });

  test('the owner can delete a goal after confirming; it then disappears', async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);
    await page.locator('#delete').click();
    await expect(page).toHaveURL(new RegExp(`/Goal/Delete/${goal.id}$`));
    await expect(page.locator('.alert-error')).toContainText('Are you sure you want to delete this?');
    await expect(page.locator('.alert-error')).toContainText(goal.name);
    await page.getByRole('button', { name: 'Delete' }).click();

    await expect(page).toHaveURL(/\/$/);
    const res = await page.goto(`/Goal/Index/${goal.id}`);
    expect(res!.status()).toBe(404);
  });

  test('cancel on the delete confirmation keeps the goal', async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);
    await page.goto(`/Goal/Delete/${goal.id}`);
    await page.getByRole('button', { name: 'Cancel' }).click();
    await expect(page).toHaveURL(new RegExp(`/Goal/Index/${goal.id}$`));
    await expect(page.locator('h4', { hasText: goal.name })).toBeVisible();
  });

  test('a non-existent goal returns 404', async ({ page }) => {
    await register(page, 'g');
    const res = await page.goto('/Goal/Index/999999');
    expect(res!.status()).toBe(404);
  });
});

test.describe('Goals: lists', () => {
  test("a created goal appears in the Goals list and on the user's home page", async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);

    await page.goto('/Goal/GoalList');
    await expect(page.getByRole('link', { name: goal.name }).first()).toBeVisible();
    await expect(page.getByText('0 Supporters').first()).toBeVisible();

    await page.goto('/');
    await expect(page.getByRole('link', { name: goal.name }).first()).toBeAttached();
  });

  test('the Goals list can be filtered to "My Goals" and sorted by popularity', async ({ page, browser }) => {
    const other = await newUserSession(browser, 'go');
    const otherGoal = await createGoal(other.page);

    await register(page, 'g');
    const mine = await createGoal(page);

    await page.goto('/Goal/GoalList');
    await page.locator('#filterby').selectOption({ label: 'My Goals' });
    await expect(page.getByRole('link', { name: mine.name }).first()).toBeVisible();
    await expect(page.getByRole('link', { name: otherGoal.name })).toHaveCount(0);

    await page.locator('#sortby').selectOption({ label: 'Popularity' });
    await expect(page.getByRole('link', { name: mine.name }).first()).toBeVisible();
    await other.context.close();
  });

  test("a private goal is hidden from other users' \"All\" goal list", async ({ page, browser }) => {
    const owner = await newUserSession(browser, 'gp');
    const priv = await createGoal(owner.page, { isPrivate: true });
    const pub = await createGoal(owner.page);

    await register(page, 'g');
    await page.goto('/Goal/GoalList');
    await expect(page.getByRole('link', { name: pub.name }).first()).toBeVisible();
    await expect(page.getByRole('link', { name: priv.name })).toHaveCount(0);
    await owner.context.close();
  });

  test("My Goals page lists the user's goals", async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);
    await page.goto('/Goal/MyGoal');
    await expect(page.getByText('MY GOALS')).toBeVisible();
    await expect(page.getByRole('link', { name: goal.name }).first()).toBeVisible();
  });

  // current behavior (possible bug): the legacy list action passes the wrong model type to its view
  test('/Goal/ListOfGoals currently fails with a server error', async ({ page }) => {
    await register(page, 'g');
    const res = await page.goto('/Goal/ListOfGoals');
    expect(res!.status()).toBe(500);
    await expect(page.locator('body')).toContainText('GoalsPageViewModel');
  });
});

test.describe('Goals: following', () => {
  test('another user can follow and unfollow a goal; the owner sees the follower', async ({ page, browser }) => {
    const owner = await newUserSession(browser, 'gf');
    const goal = await createGoal(owner.page);

    await register(page, 'g');
    await page.goto(`/Goal/Index/${goal.id}`);
    await expect(page.locator('#editBtn')).toHaveCount(0); // not the owner
    await page.locator('#SupportBtn').click();
    await expect(page.locator(`#UnSupportBtn${goal.id}`)).toBeVisible();

    await page.goto('/Goal/FollowedGoal');
    await expect(page.getByRole('link', { name: goal.name }).first()).toBeVisible();

    await owner.page.goto(`/Goal/Index/${goal.id}`);
    await expect(owner.page.locator('#divSupport')).toContainText('1 Follower');

    await page.goto(`/Goal/Index/${goal.id}`);
    await page.locator('#UnSupportBtn').click();
    await expect(page.locator(`#SupportBtn${goal.id}`)).toBeVisible();
    await owner.context.close();
  });

  test('the owner can invite a registered user to follow a goal, who then accepts from notifications', async ({
    page,
    browser,
  }) => {
    const invitee = await newUserSession(browser, 'gi');
    // the invite search matches first/last name or email (not the user name)
    const first = `Inv${uniq()}`;
    await setBasicInfo(invitee.page, invitee.user.id, { first, last: 'Person', email: `${first}@example.com` });
    await register(page, 'g');
    const goal = await createGoal(page);

    await inviteViaSearch(page, 'goal', goal.id, invitee.user, first);

    await invitee.page.goto('/Notification/Index');
    const card = invitee.page.locator('.alert-info', { hasText: 'invited you to follow the goal' }).last();
    await expect(card).toContainText(goal.name);
    await card.getByRole('link', { name: 'Follow', exact: true }).click();
    await expect(invitee.page).toHaveURL(new RegExp(`/Goal/Index/${goal.id}$`));
    await invitee.context.close();
  });

  // fixed in the port: the original view did not compile (used ApplicationUser.UserId)
  test('the supporters page for a goal renders', async ({ page }) => {
    await register(page, 'g');
    const goal = await createGoal(page);
    const res = await page.goto(`/Goal/Supporters/${goal.id}`);
    expect(res!.status()).toBe(200);
    await expect(page.getByRole('heading', { name: 'Supporters' })).toBeVisible();
  });
});

test.describe('Goal updates and progress', () => {
  test('the owner can post an update with a status and sees the status message', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await postUpdate(page, 'Ran my first 10k', '10');
    await expect(page.locator('#divupdate')).toContainText('10 Km achieved out of 100 Km');
    await expect(page.locator('#divupdate')).toContainText('0 Supports');
    await expect(page.locator('#divupdate')).toContainText('0 Comments');
  });

  test('reaching the target shows "Target achieved"', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '50' });
    await postUpdate(page, 'Done!', '50');
    await expect(page.locator('#divupdate .goal-status-message')).toContainText('Target achieved');
  });

  test('exceeding the target shows "Target exceeded"', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '50' });
    await postUpdate(page, 'Overshot', '60');
    await expect(page.locator('#divupdate .goal-status-message')).toContainText('exceeded');
    await expect(page.locator('#divupdate .goal-status-message')).toContainText('10 Km');
  });

  test('posting an empty update shows a validation message', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await page.locator('#postupdate').click();
    await expect(page.locator('#UpdateValidation')).toContainText('Please Enter Update');
  });

  test('a non-numeric status is rejected on the client', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await page.locator('#Update').fill('text');
    await page.locator('#Status').fill('lots');
    await page.locator('#postupdate').click();
    await expect(page.locator('#statusError')).toContainText('Enter a valid status.');
  });

  test('a new status lower than an earlier one is not accepted', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await postUpdate(page, 'first', '40');
    await page.locator('#Update').fill('going backwards');
    await page.locator('#Status').fill('10');
    await page.locator('#postupdate').click();
    await expect(page.locator('#divupdate')).toContainText('first');
    await expect(page.locator('#divupdate')).not.toContainText('going backwards');
  });

  // current behavior (possible bug): the Post button only renders when the goal has a metric,
  // so a goal without a metric cannot receive any updates.
  test('a goal without a metric has no way to post updates', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page);
    await expect(page.locator('#Status')).toHaveCount(0);
    await expect(page.locator('#postupdate')).toHaveCount(0);
  });

  test('the owner can edit an update', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await postUpdate(page, 'original text', '10');
    await page.locator('#editupdate').click();
    await page.locator('#divupdate textarea').fill('edited text');
    await page.locator('#posteditupdate').click();
    await expect(page.locator('#divupdate')).toContainText('edited text');
    await expect(page.locator('#divupdate')).not.toContainText('original text');
  });

  test('the owner can delete an update after confirming', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await postUpdate(page, 'to be deleted', '10');
    await page.locator('#deleteupdate').click();
    await expect(page.locator('#divupdate')).toContainText('Are you sure, you want to delete this update..?');
    await page.locator('#divupdate').getByRole('button', { name: 'Delete' }).click();
    await expect(page.locator('#divupdate')).toContainText('No updates on this goal');
  });

  test('users can support an update and comment on it', async ({ page, browser }) => {
    const owner = await newUserSession(browser, 'uo');
    const goal = await createGoal(owner.page, { metric: 'Km', target: '100' });
    await postUpdate(owner.page, 'owner update', '5');

    await register(page, 'u');
    await page.goto(`/Goal/Index/${goal.id}`);
    await page.locator('a[onclick^="SupportUpdate"]').click();
    await expect(page.locator('[id^="SupportCount"]').first()).toContainText('1 Supports');

    await page.locator('[id^="CommentText"]').fill('Nice work!');
    await page.locator('[id^="postComment"]').click();
    await expect(page.locator('[id^="divcommentUpdate"]').first()).toContainText('Nice work!');
    await expect(page.locator('[id^="commentToggle"]').first()).toContainText('1 Comments');
    await owner.context.close();
  });

  test('the owner can change the goal status to Completed, which hides the update box', async ({ page }) => {
    await register(page, 'u');
    await createGoal(page, { metric: 'Km', target: '100' });
    await page.locator('#GoalStatusId').selectOption({ label: 'Completed' });
    await page.locator('#drpdwnBtn').click();
    await expect(page.locator('#goalStatus')).toHaveText('Completed');
    await expect(page.locator('#UpdatePostContent')).toBeHidden();
  });
});
