import { Browser, BrowserContext, Page, expect } from '@playwright/test';

export const PASSWORD = 'Passw0rd!x';

let counter = 0;
/** Short unique suffix: time + counter + random, safe across reruns. */
export function uniq(): string {
  counter += 1;
  return `${Date.now().toString(36)}${counter}${Math.random().toString(36).slice(2, 5)}`;
}

export interface User {
  userName: string;
  password: string;
  id: string;
}

/** Register through the UI. Leaves the page signed in on the home page. */
export async function register(page: Page, prefix = 'u'): Promise<User> {
  const userName = `${prefix}${uniq()}`;
  await page.goto('/Account/Register');
  await page.locator('#UserName').fill(userName);
  await page.locator('#Password').fill(PASSWORD);
  await page.locator('#ConfirmPassword').fill(PASSWORD);
  await page.getByRole('button', { name: 'Register' }).click();
  await expect(page).toHaveURL(/\/$/);
  return { userName, password: PASSWORD, id: await currentUserId(page) };
}

/** User id taken from the "Profile" link in the navbar. */
export async function currentUserId(page: Page): Promise<string> {
  const href = await page
    .locator('a[href^="/Account/UserProfile/"]', { hasText: 'Profile' })
    .first()
    .getAttribute('href');
  return href!.split('/').pop()!;
}

export async function login(page: Page, userName: string, password = PASSWORD): Promise<void> {
  await page.goto('/Account/Login');
  await page.locator('#Email').fill(userName); // field is called Email but takes the user name
  await page.locator('#Password').fill(password);
  await page.getByRole('button', { name: 'Log in' }).click();
}

export async function logout(page: Page): Promise<void> {
  await page.goto('/Account/LogOff');
}

/** A fresh browser context (own cookies) with a newly registered user. */
export async function newUserSession(
  browser: Browser,
  prefix = 'u',
): Promise<{ page: Page; context: BrowserContext; user: User }> {
  const context = await browser.newContext();
  const page = await context.newPage();
  const user = await register(page, prefix);
  return { page, context, user };
}

/** Readonly datepicker inputs: set the value directly. Format is dd-MMM-yyyy. */
export async function setDate(page: Page, selector: string, value: string): Promise<void> {
  await page.$eval(
    selector,
    (el, v) => {
      el.removeAttribute('readonly');
      (el as HTMLInputElement).value = v as string;
    },
    value,
  );
}

export interface GoalOptions {
  name?: string;
  desc?: string;
  start?: string;
  end?: string;
  metric?: string; // e.g. 'Km'
  target?: string;
  isPrivate?: boolean;
}

/** Create a goal through /Goal/Create. Returns the goal name and id; ends on /Goal/Index/{id}. */
export async function createGoal(page: Page, o: GoalOptions = {}): Promise<{ name: string; id: string }> {
  const name = o.name ?? `Goal ${uniq()}`;
  await page.goto('/Goal/Create');
  await page.locator('#GoalName').fill(name);
  await page.locator('#Desc').fill(o.desc ?? 'A goal created by the e2e suite');
  await setDate(page, '#StartDate', o.start ?? '01-Jan-2026');
  await setDate(page, '#EndDate', o.end ?? '31-Dec-2030');
  if (o.metric) await page.locator('#MetricId').selectOption({ label: o.metric });
  if (o.target) await page.locator('#Target').fill(o.target);
  if (o.isPrivate) await page.locator('#GoalType').check();
  await page.locator('#submitbtn').click();
  await expect(page).toHaveURL(/\/Goal\/Index\/\d+/);
  return { name, id: page.url().split('/').pop()! };
}

/** Post a goal update on the goal page (as the goal owner). */
export async function postUpdate(page: Page, text: string, status?: string): Promise<void> {
  await page.locator('#Update').fill(text);
  if (status !== undefined) await page.locator('#Status').fill(status);
  await page.locator('#postupdate').click();
  await expect(page.locator('#divupdate')).toContainText(text);
}

/** Create a group through /Group/CreateGroup. Ends on /Group/Index/{id}. */
export async function createGroup(
  page: Page,
  name = `Group ${uniq()}`,
  description = 'A group created by the e2e suite',
): Promise<{ name: string; id: string }> {
  await page.goto('/Group/CreateGroup');
  await page.locator('#InputGroup').fill(name);
  await page.locator('#InputGroupDesc').fill(description);
  await page.locator('#CreateGroup').click();
  await expect(page).toHaveURL(/\/Group\/Index\/\d+/);
  return { name, id: page.url().split('/').pop()! };
}

/**
 * Fill in the "Basic Info" of the signed-in user via the profile page (first name, last name, email).
 * Needed by user search/invite flows: those match on first/last name or email, not on the user name.
 */
export async function setBasicInfo(
  page: Page,
  userId: string,
  info: { first: string; last: string; email: string },
): Promise<void> {
  await page.goto(`/Account/UserProfile/${userId}`);
  await page.locator('#editprofile').click();
  await page.locator('#divBasicInfo #FirstName').fill(info.first);
  await page.locator('#divBasicInfo #LastName').fill(info.last);
  await page.locator('#divBasicInfo #Email').fill(info.email);
  await page.locator('#divBasicInfo').getByRole('button', { name: 'Save' }).click();
  await expect(page).toHaveURL(new RegExp(`/Account/UserProfile/${userId}$`));
}

/**
 * Invite a user the way the invite box does after a suggestion is picked: first query the search
 * endpoint (like the autocomplete does), then POST the chosen user id.
 * The jQuery-UI autocomplete widget itself is not driven here: on the group invite page its key handler
 * fires an alert("Entered") on every keystroke and the suggestion menu does not render reliably.
 */
export async function inviteViaSearch(
  page: Page,
  kind: 'goal' | 'group',
  id: string,
  invitee: { id: string; userName: string },
  searchTerm: string,
): Promise<void> {
  const searchUrl =
    kind === 'goal'
      ? `/Goal/SearchUser?username=${encodeURIComponent(searchTerm)}&goalId=${id}`
      : `/Group/SearchUserForGroup?username=${encodeURIComponent(searchTerm)}&groupId=${id}`;
  const postUrl = kind === 'goal' ? '/Goal/InviteUser' : '/Group/InviteUser';
  const found = await page.evaluate(async (u) => (await fetch(u)).json(), searchUrl);
  expect(found.map((x: { id: string }) => x.id)).toContain(invitee.id);
  const html = await page.evaluate(
    async ([u, groupOrGoalId, userId]) => {
      const body = new URLSearchParams({ id: groupOrGoalId, UserId: userId });
      const r = await fetch(u, { method: 'POST', body, headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      return r.text();
    },
    [postUrl, id, invitee.id],
  );
  expect(html).toContain(invitee.userName);
}
