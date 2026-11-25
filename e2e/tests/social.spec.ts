import { test, expect } from '@playwright/test';
import { createGoal, createGroup, inviteViaSearch, newUserSession, register, setBasicInfo, uniq } from '../helpers/app';

test.describe('Follow requests between users', () => {
  test('following a user sends a request that the other user accepts from notifications', async ({ page, browser }) => {
    const target = await newUserSession(browser, 'ft');
    const me = await register(page, 'fr');

    await page.goto(`/Account/UserProfile/${target.user.id}`);
    await expect(page.getByText(`Follow ${target.user.userName} to see the updates`)).toBeVisible();
    await page.getByRole('link', { name: 'Follow', exact: true }).click();
    await expect(page.getByText('Your Following request is waiting for approval.').first()).toBeVisible();

    await target.page.goto('/Notification/Index');
    const card = target.page.locator('.alert-info', { hasText: 'wants to Follow you.' }).last();
    await expect(card).toContainText(me.userName);
    await card.getByRole('link', { name: 'Accept' }).click();
    await expect(target.page).toHaveURL(/\/Notification(\/Index)?$/);
    await expect(target.page.getByText('wants to Follow you.')).toHaveCount(0);

    // the requester now follows the user and can unfollow
    await page.goto(`/Account/UserProfile/${target.user.id}`);
    await expect(page.getByRole('link', { name: 'Unfollow' })).toBeVisible();

    await page.goto('/Account/Followings');
    await expect(page.getByRole('link', { name: target.user.userName })).toBeVisible();
    await target.page.goto('/Account/Followers');
    await expect(target.page.getByRole('link', { name: me.userName })).toBeVisible();

    await page.goto(`/Account/UserProfile/${target.user.id}`);
    await page.getByRole('link', { name: 'Unfollow' }).click();
    await expect(page.getByRole('link', { name: 'Follow', exact: true })).toBeVisible();
    await target.context.close();
  });

  test('a rejected follow request disappears and the requester can send a new one', async ({ page, browser }) => {
    const target = await newUserSession(browser, 'ft');
    const me = await register(page, 'fr');
    await page.goto(`/Account/UserProfile/${target.user.id}`);
    await page.getByRole('link', { name: 'Follow', exact: true }).click();

    await target.page.goto('/Notification/Index');
    await target.page
      .locator('.alert-info', { hasText: 'wants to Follow you.' })
      .last()
      .getByRole('link', { name: 'Reject' })
      .click();
    await expect(target.page.getByText(`${me.userName} wants to Follow you.`)).toHaveCount(0);

    await page.goto(`/Account/UserProfile/${target.user.id}`);
    await expect(page.getByRole('link', { name: 'Follow', exact: true })).toBeVisible();
    await target.context.close();
  });

  test('the followers page shows an empty state for a new user', async ({ page }) => {
    await register(page, 'fr');
    await page.goto('/Account/Followers');
    await expect(page.getByText('No followers')).toBeVisible();
    await page.goto('/Account/Followings');
    await expect(page.locator('.well').first()).toContainText('Following');
  });
});

test.describe('Notifications / invitations', () => {
  test('a new user has no invitations', async ({ page }) => {
    await register(page, 'nt');
    await page.getByRole('link', { name: 'My Invitations' }).click();
    await expect(page).toHaveURL(/\/Notification\/Index/);
    await expect(page.getByRole('heading', { name: 'Invitations for you' })).toBeVisible();
    await expect(page.getByText('No invitations for you.')).toBeVisible();
  });

  test('group invitations, goal invitations and follow requests all show up together', async ({ page, browser }) => {
    const me = await register(page, 'nt');
    const sender = await newUserSession(browser, 'ns');

    // follow request from the sender
    await sender.page.goto(`/Account/UserProfile/${me.id}`);
    await sender.page.getByRole('link', { name: 'Follow', exact: true }).click();

    // group + goal invitations from the sender (invite search matches first name / email)
    const first = `Me${uniq()}`;
    await setBasicInfo(page, me.id, { first, last: 'Person', email: `${first}@example.com` });
    const group = await createGroup(sender.page);
    const goal = await createGoal(sender.page);
    await inviteViaSearch(sender.page, 'group', group.id, me, first);
    await inviteViaSearch(sender.page, 'goal', goal.id, me, first);

    await page.goto('/Notification/Index');
    await expect(page.locator('.alert-info', { hasText: 'Invited you to join' }).last()).toContainText(group.name);
    await expect(page.locator('.alert-info', { hasText: 'invited you to follow the goal' }).last()).toContainText(
      goal.name,
    );
    await expect(page.locator('.alert-info', { hasText: 'wants to Follow you.' }).last()).toContainText(
      sender.user.userName,
    );
    await sender.context.close();
  });
});

test.describe('Email invitations', () => {
  // The invite e-mail itself cannot be observed (no SMTP is configured and the token link only exists in the
  // mail), so this suite checks what the UI reports for the invite box. Accepting a token link is not covered.
  test('the group invite e-mail button is enabled only while the e-mail box has focus', async ({ page }) => {
    await register(page, 'em');
    const group = await createGroup(page);
    await page.goto(`/Group/InviteUsers/${group.id}`);
    await expect(page.locator('#btnInvite')).toBeDisabled();
    await page.locator('#Email').focus();
    await expect(page.locator('#btnInvite')).toBeEnabled();
  });

  test('inviting an e-mail address posts to /Group/InviteEmail', async ({ page }) => {
    await register(page, 'em');
    const group = await createGroup(page);
    await page.goto(`/Group/InviteUsers/${group.id}`);
    const response = page.waitForResponse((r) => r.url().includes('/Group/InviteEmail'));
    await page.locator('#Email').fill(`${uniq()}@example.com`);
    await page.locator('#btnInvite').click();
    const res = await response;
    // current behavior (possible bug): with no SMTP server configured the mail send can fail, in which case
    // the endpoint answers 500 instead of "Invitation Sent!". Either outcome is accepted here.
    expect([200, 500]).toContain(res.status());
    if (res.status() === 200) {
      await expect(page.locator('#msgGroup')).toHaveText('Invitation Sent!');
    }
  });

  // current behavior (possible bug): the e-mail accept endpoint reads a one-off token from TempData;
  // opened directly (no token) it throws instead of redirecting home.
  test('/EmailRequest/AddGroupUser without an invitation token fails with a server error', async ({ page }) => {
    await register(page, 'em');
    const res = await page.goto('/EmailRequest/AddGroupUser');
    expect(res!.status()).toBe(500);
  });

  test('a login link carrying an invitation guid still shows the login page', async ({ page }) => {
    await page.goto('/Account/Login?guid=gr:00000000-0000-0000-0000-000000000000');
    await expect(page).toHaveURL(/\/Account\/Login\?guid=/);
    await expect(page.locator('#loginForm')).toBeVisible();
  });
});
