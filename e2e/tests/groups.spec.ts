import { test, expect } from '@playwright/test';
import { createGroup, newUserSession, inviteViaSearch, register, setBasicInfo, setDate, uniq } from '../helpers/app';

test.describe('Groups: create / edit / delete', () => {
  test('a user can create a group and becomes its admin and first member', async ({ page }) => {
    const user = await register(page, 'gr');
    const group = await createGroup(page, `Group ${uniq()}`, 'We run together');

    await expect(page.locator('h4', { hasText: group.name })).toBeVisible();
    await expect(page.locator('.well').first()).toContainText('We run together');
    await expect(page.locator('#groupEdit')).toBeVisible();
    await expect(page.locator('#groupDelete')).toBeVisible();
    await expect(page.locator('#memberlistcontainer')).toContainText(user.userName);
    await expect(page.getByRole('button', { name: /Requests/ })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Actions' })).toBeVisible();
  });

  test('creating a group without a name is rejected', async ({ page }) => {
    await register(page, 'gr');
    await page.goto('/Group/CreateGroup');
    await page.locator('#CreateGroup').click();
    await expect(page).toHaveURL(/\/Group\/CreateGroup/);
  });

  test('group names must be unique', async ({ page }) => {
    await register(page, 'gr');
    const group = await createGroup(page);
    await page.goto('/Group/CreateGroup');
    await page.locator('#InputGroup').fill(group.name);
    await page.locator('#InputGroupDesc').fill('dup');
    await page.locator('#CreateGroup').click();
    await expect(page).toHaveURL(/\/Group\/CreateGroup/);
    await expect(page.locator('body')).toContainText('Group already exists');
  });

  test('the admin can edit a group', async ({ page }) => {
    await register(page, 'gr');
    const group = await createGroup(page);
    await page.locator('#groupEdit').click();
    await expect(page).toHaveURL(new RegExp(`/Group/EditGroup/${group.id}$`));
    const newName = `${group.name} renamed`;
    await page.locator('#GroupName').fill(newName);
    await page.locator('#InputGroupDesc').fill('Edited group description');
    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));
    await expect(page.locator('h4', { hasText: newName })).toBeVisible();
    await expect(page.locator('.well').first()).toContainText('Edited group description');
  });

  test('the admin can delete a group after confirming', async ({ page }) => {
    await register(page, 'gr');
    const group = await createGroup(page);
    await page.locator('#groupDelete').click();
    await expect(page).toHaveURL(new RegExp(`/Group/DeleteGroup/${group.id}$`));
    await page.getByRole('button', { name: 'Delete' }).click();
    await expect(page).toHaveURL(/\/$/);

    await page.goto('/Group/GroupList');
    await expect(page.getByRole('link', { name: group.name })).toHaveCount(0);
  });

  test('a created group shows up in the group list and under My Groups on the home page', async ({ page }) => {
    await register(page, 'gr');
    const group = await createGroup(page);
    await page.goto('/Group/GroupList');
    await expect(page.getByRole('link', { name: group.name }).first()).toBeVisible();
    await page.locator('#Filter').selectOption({ label: 'My Groups' });
    await expect(page.getByRole('link', { name: group.name }).first()).toBeVisible();
    await page.goto('/Group/MyGroups');
    await expect(page.getByRole('link', { name: group.name }).first()).toBeVisible();
  });

  // current behavior (possible bug): legacy list action passes the wrong model type to its view
  test('/Group/ListOfGroups currently fails with a server error', async ({ page }) => {
    await register(page, 'gr');
    const res = await page.goto('/Group/ListOfGroups');
    expect(res!.status()).toBe(500);
    await expect(page.locator('body')).toContainText('GroupsPageViewModel');
  });

  // current behavior (possible bug): /Group has no default Index id, so the bare URL is a server error
  test('/Group/Index without an id currently fails with a server error', async ({ page }) => {
    await register(page, 'gr');
    const res = await page.goto('/Group/Index');
    expect(res!.status()).toBe(500);
  });
});

test.describe('Groups: joining, requests and permissions', () => {
  test('a non-member sees "Join Now", no admin controls and no clickable goals', async ({ page, browser }) => {
    const admin = await newUserSession(browser, 'ga');
    const group = await createGroup(admin.page);

    await register(page, 'gm');
    await page.goto(`/Group/Index/${group.id}`);
    await expect(page.getByRole('link', { name: 'Join Now' })).toBeVisible();
    await expect(page.locator('#groupEdit')).toHaveCount(0);
    await expect(page.locator('#groupDelete')).toHaveCount(0);
    await expect(page.getByRole('button', { name: 'Actions' })).toHaveCount(0);
    await admin.context.close();
  });

  test('a user can request to join; the admin accepts, then the user is a member', async ({ page, browser }) => {
    const admin = await newUserSession(browser, 'ga');
    const group = await createGroup(admin.page);

    const member = await register(page, 'gm');
    await page.goto(`/Group/Index/${group.id}`);
    await page.getByRole('link', { name: 'Join Now' }).click();
    await expect(page.locator('.alert-success')).toContainText('Your joining request is waiting for approval from group admin.');

    await admin.page.goto(`/Group/ShowAllRequests/${group.id}`);
    const row = admin.page.locator('.alert-info', { hasText: member.userName });
    await expect(row).toContainText('wants to join');
    await row.getByRole('link', { name: 'Accept' }).click();
    await expect(admin.page.locator('body')).toContainText('No results to display');

    await page.goto(`/Group/Index/${group.id}`);
    await expect(page.locator('#memberlistcontainer')).toContainText(member.userName);
    await expect(page.getByRole('button', { name: 'Actions' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Join Now' })).toHaveCount(0);
    await admin.context.close();
  });

  test('the admin can reject a join request', async ({ page, browser }) => {
    const admin = await newUserSession(browser, 'ga');
    const group = await createGroup(admin.page);

    const member = await register(page, 'gm');
    await page.goto(`/Group/Index/${group.id}`);
    await page.getByRole('link', { name: 'Join Now' }).click();

    await admin.page.goto(`/Group/ShowAllRequests/${group.id}`);
    await admin.page.locator('.alert-info', { hasText: member.userName }).getByRole('link', { name: 'Reject' }).click();
    await expect(admin.page.locator('body')).toContainText('No results to display');

    await page.goto(`/Group/Index/${group.id}`);
    await expect(page.getByRole('link', { name: 'Join Now' })).toBeVisible();
    await admin.context.close();
  });

  // current behavior (possible bug): the member-list "Unfollow" link passes only {id} to DeleteMember,
  // which needs userId and groupId, so leaving a group this way fails with a server error.
  test('the "Unfollow" link in the member list does not let a member leave (server error)', async ({ page, browser }) => {
    const admin = await newUserSession(browser, 'ga');
    const group = await createGroup(admin.page);
    const member = await register(page, 'gm');
    await page.goto(`/Group/Index/${group.id}`);
    await page.getByRole('link', { name: 'Join Now' }).click();
    await admin.page.goto(`/Group/ShowAllRequests/${group.id}`);
    await admin.page.locator('.alert-info', { hasText: member.userName }).getByRole('link', { name: 'Accept' }).click();

    await page.goto(`/Group/Index/${group.id}`);
    await page.locator('#memberlistcontainer').getByRole('link', { name: 'Unfollow' }).click();
    await expect(page.locator('body')).toContainText('Server Error');
    await page.goto(`/Group/Index/${group.id}`);
    await expect(page.locator('#memberlistcontainer')).toContainText(member.userName);
    await admin.context.close();
  });

  test('the admin can invite a user by name; the invitee accepts from notifications', async ({ page, browser }) => {
    const invitee = await newUserSession(browser, 'gi');
    // the invite search matches first/last name or email (not the user name)
    const first = `Inv${uniq()}`;
    await setBasicInfo(invitee.page, invitee.user.id, { first, last: 'Person', email: `${first}@example.com` });
    const admin = await register(page, 'ga');
    const group = await createGroup(page);

    await page.getByRole('button', { name: 'Actions' }).click();
    await page.getByRole('link', { name: 'Invite People' }).click();
    await expect(page).toHaveURL(new RegExp(`/Group/InviteUsers/${group.id}$`));
    await inviteViaSearch(page, 'group', group.id, invitee.user, first);

    await invitee.page.goto('/Notification/Index');
    const card = invitee.page.locator('.alert-info', { hasText: 'Invited you to join' }).last();
    await expect(card).toContainText(admin.userName);
    await expect(card).toContainText(group.name);
    await card.getByRole('link', { name: 'Join Now' }).click();
    await expect(invitee.page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));
    await expect(invitee.page.locator('#memberlistcontainer')).toContainText(invitee.user.userName);
    await invitee.context.close();
  });

  test('the admin can remove a member', async ({ page, browser }) => {
    const admin = await newUserSession(browser, 'ga');
    const group = await createGroup(admin.page);
    const member = await register(page, 'gm');
    await page.goto(`/Group/Index/${group.id}`);
    await page.getByRole('link', { name: 'Join Now' }).click();
    await admin.page.goto(`/Group/ShowAllRequests/${group.id}`);
    await admin.page.locator('.alert-info', { hasText: member.userName }).getByRole('link', { name: 'Accept' }).click();

    await admin.page.goto(`/Group/Index/${group.id}`);
    await admin.page.locator('#memberlistcontainer a[title], #memberlistcontainer a#delete').first().click();
    await admin.page.goto(`/Group/Index/${group.id}`);
    await expect(admin.page.locator('#memberlistcontainer')).not.toContainText(member.userName);
    await admin.context.close();
  });
});

test.describe('Groups: focus areas and goals', () => {
  test('a member can create, edit and delete a focus area', async ({ page }) => {
    await register(page, 'gf');
    const group = await createGroup(page);
    const focus = `Focus ${uniq()}`;

    await page.goto(`/Group/CreateFocus/${group.id}`);
    await page.locator('#FocusName').fill(focus);
    await page.locator('#Description').fill('Cardio');
    await page.getByRole('button', { name: 'Create' }).click();
    await expect(page).toHaveURL(/\/Group\/Focus\/\d+/);
    // current behavior (possible bug): the focus is saved, but the page it redirects to does not compile
    await expect(page.locator('body')).toContainText('Compilation Error');

    await page.goto(`/Group/Index/${group.id}`);
    await expect(page.locator('#focuslistcontainer')).toContainText(focus);

    await page.locator('#focuslistcontainer a#edit').first().click();
    await page.locator('#FocusName').fill(`${focus} v2`);
    await page.getByRole('button', { name: /Save|Update/ }).click();
    await expect(page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));
    await expect(page.locator('#focuslistcontainer')).toContainText(`${focus} v2`);

    await page.locator('#focuslistcontainer a#delete').first().click();
    await page.getByRole('button', { name: 'Delete' }).click();
    await expect(page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));
    await expect(page.locator('#focuslistcontainer')).not.toContainText(`${focus} v2`);
  });

  test('a member can create and delete a group goal (editing is broken, see below)', async ({ page }) => {
    const user = await register(page, 'gg');
    const group = await createGroup(page);
    const goalName = `Group goal ${uniq()}`;

    // the Actions dropdown holds the link to the create-goal page
    await expect(page.locator(`a[href="/Group/CreateGoal/${group.id}"]`)).toBeAttached();
    await page.goto(`/Group/CreateGoal/${group.id}`);
    await expect(page).toHaveURL(new RegExp(`/Group/CreateGoal/${group.id}$`));
    await page.locator('#GoalName').fill(goalName);
    await page.locator('#Description').fill('Team goal');
    await setDate(page, '#StartDate', '01-Jan-2026');
    await setDate(page, '#EndDate', '31-Dec-2030');
    await page.locator('#MetricId').selectOption({ label: 'Km' });
    await page.locator('#Target').fill('200');
    await page.getByRole('button', { name: 'Create' }).click();

    await expect(page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));
    const goalLink = page.locator('#goallistcontainer').getByRole('link', { name: goalName });
    await expect(goalLink).toBeVisible();
    await expect(page.locator('#goallistcontainer')).toContainText(`Created By ${user.userName}`);

    await goalLink.click();
    await expect(page).toHaveURL(/\/Group\/GroupGoal\/\d+/);
    await expect(page.locator('h4', { hasText: goalName })).toBeVisible();

    // current behavior (possible bug): the group-goal edit page does not compile, so group goals cannot be edited
    await page.locator('#editBtn').click();
    await expect(page).toHaveURL(/\/Group\/EditGoal\/\d+/);
    await expect(page.locator('body')).toContainText('Compilation Error');
    await page.goBack();

    await page.locator('#delete').click();
    await page.getByRole('button', { name: 'Delete' }).click();
    await expect(page).toHaveURL(new RegExp(`/Group/Index/${group.id}$`));
    await expect(page.locator('#goallistcontainer')).not.toContainText(goalName);
  });

  test('group goal updates can be posted, supported and commented on', async ({ page }) => {
    await register(page, 'gu');
    const group = await createGroup(page);
    const goalName = `Group goal ${uniq()}`;
    await page.goto(`/Group/CreateGoal/${group.id}`);
    await page.locator('#GoalName').fill(goalName);
    await page.locator('#Description').fill('Team goal');
    await setDate(page, '#StartDate', '01-Jan-2026');
    await setDate(page, '#EndDate', '31-Dec-2030');
    await page.locator('#MetricId').selectOption({ label: 'Km' });
    await page.locator('#Target').fill('100');
    await page.getByRole('button', { name: 'Create' }).click();
    await page.locator('#goallistcontainer').getByRole('link', { name: goalName }).click();

    await page.locator('#GroupUpdate').fill('Team update one');
    await page.locator('#Status').fill('25');
    await page.locator('#postGroupupdate').click();
    await expect(page.locator('#divgroupupdate')).toContainText('Team update one');
    await expect(page.locator('#divgroupupdate')).toContainText('25 Km achieved out of 100 Km');

    await page.locator('#divgroupupdate a[onclick^="SupportGroupUpdate"]').click();
    await expect(page.locator('#divgroupupdate [id^="SupportGroupCount"]').first()).toContainText('1 Supports');

    await page.locator('#divgroupupdate [id^="GroupCommentText"]').fill('Go team');
    await page.locator('#divgroupupdate [id^="postComment"]').click();
    await expect(page.locator('#divgroupupdate [id^="divgroupcommentUpdate"]').first()).toContainText('Go team');
  });
});

