# SocialGoal end-to-end tests (Playwright)

User-centric browser tests that describe the behavior of the **original** SocialGoal app (ASP.NET MVC 5, .NET Framework 4.8).
They are meant to be run first against the original app (the "before" baseline) and later, against the
ported .NET 10 app (set `BASE_URL`). On this branch three expectations changed for views that did not compile
in the original; see the root README.

## Prerequisites

- Node.js LTS (`winget install OpenJS.NodeJS.LTS`)
- The app running and reachable (see below)

```powershell
cd e2e
npm install
npx playwright install chromium
```

## Starting the app (the suite does NOT start it)

`playwright.config.ts` deliberately has **no `webServer` block**: the app is started by hand.

1. Open `source\SocialGoal.sln` in Visual Studio and run the `SocialGoal` web project (IIS Express, `http://localhost:5002/`).
2. SQL Server on the default local instance must be reachable. On first run the app creates and seeds the `SocialGoal`
   database itself (`GoalsSampleData`).
3. Check: `Invoke-WebRequest http://localhost:5002/ -UseBasicParsing` answers (200, after a redirect to the login page).

## Running

```powershell
npx playwright test                          # everything (Chromium, headless, one worker)
npx playwright test tests/goals.spec.ts      # one spec
npx playwright test -g "invite"              # by test name
npx playwright test --headed                 # watch it
npx playwright show-report                   # open the HTML report of the last run
$env:BASE_URL = "https://localhost:5001"; npx playwright test   # another host (e.g. the ported app)
```

`BASE_URL` defaults to `http://localhost:5002`. Traces are recorded on the first retry (use `--retries=1`),
screenshots on failure.

## Database state assumptions

- There is **no predefined user**: every test registers its own users through the UI with unique names
  (`<prefix><time><counter><random>`), and creates its own goals/groups with unique names. Reruns never collide and no
  cleanup is needed.
- The suite never assumes an empty database; it only looks at data it created itself. It does leave that data behind
  (users, goals, groups). Drop the `SocialGoal` database (the app recreates it) if you want a clean slate.
- The only data assumed to exist is the seeded lookup data: the metrics (`$`, `$ M`, `%`, `Hours`, `Kg`, `Km`, `Rs`,
  `Years`) and the goal statuses (`Completed`, `In Progress`, `On Hold`).
- Tests are independent and can run in any order. `workers` is 1 to keep a shared database and failures easy to read.

## Layout

| Path | Content |
| --- | --- |
| `playwright.config.ts` | Chromium only, `BASE_URL`, trace on first retry, no webServer |
| `helpers/app.ts` | UI-only setup helpers: `register`, `login`, `logout`, `newUserSession` (own browser context), `createGoal`, `postUpdate`, `createGroup`, `setBasicInfo`, `inviteViaSearch` |
| `tests/auth.spec.ts` | Register, login/logout, failed login, validation errors, external-login buttons |
| `tests/access-control.spec.ts` | Anonymous users are redirected to the login page for protected pages |
| `tests/goals.spec.ts` | Goal create/view/edit/delete, lists and filters, following, updates, progress, comments, status |
| `tests/groups.spec.ts` | Group create/edit/delete, join requests, invite, members, focus areas, group goals and updates |
| `tests/search.spec.ts` | Search for goals, people and groups |
| `tests/social.spec.ts` | Follow requests, notifications/invitations, e-mail invitation box |
| `tests/profile.spec.ts` | Profile view, edit basic/personal info, picture upload |

## Covered / intentionally skipped

- External logins (Facebook/Google/Microsoft/Twitter): only the rendered button(s) are asserted (only Google is configured).
- E-mail invitation links: sending needs SMTP (none configured) and the token link only exists in the mail, so accepting an
  e-mail invitation is not covered. The invite box and the raw endpoints are.
- The jQuery-UI autocomplete of the invite boxes is not driven (the group box fires an `alert("Entered")` per keystroke and the
  suggestion menu does not render reliably). `inviteViaSearch` performs the same two requests the widget makes (search, then
  invite) from inside the page.
- Chart rendering (jqPlot) and layout/visuals.

## Side effects

The profile-picture test uploads a PNG; the app stores it under `source\SocialGoal\Content\ProfilePics\` (untracked files
appear in `git status` after each run; delete them if unwanted).

## Tests that pin down current (buggy) behavior

Tests marked `// current behavior (possible bug)` assert what the original app does today, including server errors, so
that the port can be compared on purpose. When the port fixes one of these, the test should be updated deliberately,
not silently. See the list in the final report / the `grep` below:

```powershell
Select-String -Path tests\*.ts -Pattern "possible bug"
```
