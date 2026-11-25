SocialGoal on .NET 10
=====================

SocialGoal is a social network for sharing goals and progress, originally built by
[Marlabs](http://www.marlabs.com) as a learning app for ASP.NET MVC 5 and EF 6.
This fork ports it to ASP.NET Core on .NET 10 developed and tested on Linux.

Before and after
----------------

| | Before (tag `baseline-net48`) | After |
|---|---|---|
| Runtime | .NET Framework 4.8, Windows + IIS Express | .NET 10, Kestrel, tested on Linux |
| Web | ASP.NET MVC 5, OWIN | ASP.NET Core MVC |
| Data | EF 6 Code First, database initializer | EF Core 10, migration + seed data |
| Identity | ASP.NET Identity 1.0 | ASP.NET Core Identity |
| DI / mapping | Autofac, AutoMapper | Built-in container, Mapperly |
| Mail | MvcMailer | `SmtpClient` + Razor templates |
| Images | System.Drawing | SkiaSharp |
| Errors | Elmah | Built-in logging |
| Projects | Old-style csproj, `packages.config` | SDK-style |
| Tests | NUnit 2 | NUnit 4 (all 113 original tests kept, sign-in and session setup rewritten, plus security checks) |
| Browser tests | 93 pass | 93 pass (3 expectations changed, see below) |

The front end (jQuery, Bootstrap, page scripts, URLs) is unchanged.

Running it
----------

You need the .NET 10 SDK and SQL Server (a container works):

```bash
export MSSQL_SA_PASSWORD='<choose a strong password>'
docker run -d --name socialgoal-sql -e ACCEPT_EULA=Y -e MSSQL_PID=Developer -e MSSQL_SA_PASSWORD \
  -p 127.0.0.1:1433:1433 -v socialgoal-sql:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest
export ConnectionStrings__SocialGoalEntities="Server=localhost,1433;Database=SocialGoal;User Id=sa;Password=$MSSQL_SA_PASSWORD;MultipleActiveResultSets=True;TrustServerCertificate=True"
dotnet run --project source/SocialGoal
```

Open http://localhost:5002 and register a user. The database is created and migrated on startup.

Configuration (environment variables or `dotnet user-secrets`):

| Key | Purpose |
|---|---|
| `Authentication:Google:ClientId` / `ClientSecret` | Google sign-in. Development uses placeholders; the button is hidden when empty |
| `Mail:Host`, `Port`, `UserName`, `Password`, `From` | SMTP. Without a host, mails are written to `source/SocialGoal/App_Data/mail` |
| `Mail:BaseUrl` | Public address used in mailed links. **Set this in any deployment** |

Tests:

```bash
dotnet test source/SocialGoal.sln
```

```bash
cd e2e && npm ci && npx playwright install chromium && npx playwright test
```

The browser tests need the app running. CI (`.github/workflows/ci.yml`) runs build, unit and browser tests.

Behaviour differences
---------------------

**Pages that did not compile in the original, now fixed** (`item.UserId` on a user object, and similar):
goal supporters, supporters of a goal or group update, group members, joined users, group focus page,
and group goal editing. Three browser tests expected the old compile error and now expect the working page.

**Original bugs kept on purpose** (pinned by browser tests): `/Goal/ListOfGoals`, `/Group/ListOfGroups`,
`/Group/Index` without an id and `/Account/Manage` return a server error; the member-list "Unfollow" link
fails.

**Other changes**

- Child actions (`Html.Action`) are run by a small helper (`Helpers/ChildActionExtensions.cs`) rather than
  rewritten as view components. Action filters do not run for them.
- Helper methods that were public on controllers are no longer reachable by URL.
- The error page is plain; exception messages appear only in Development.

Security
--------

Changed in the port:

- Elmah is gone. The original served its error log at `/elmah` without authentication.
- Picture upload by URL only fetches public http/https addresses, including after redirects (the original
  fetched any URL: server-side request forgery). Size and dimensions are limited, and non-images are rejected.
- Replacing a picture only deletes a previous upload.
- Mailed links use the configured address, not the request's Host header.
- ASP.NET request validation does not exist in ASP.NET Core; a replacement rejects markup in query and form values.

Known weaknesses carried over:

- Several actions change data on GET (sign out, un-support, delete member and others), and most POST actions
  do not check the anti-forgery token: **cross-site request forgery is possible**.
- Many actions do not check that the signed-in user owns the item being changed.
- No account lockout, and passwords only need 6 characters.

Not covered
-----------

- **Existing data is not migrated.** The port creates a new database; old schemas and password hashes are not converted.
- No cloud deployment, UI upgrade or new features.
- Google sign-in is configured but not tested against Google.
- Real SMTP delivery is not tested; invitation links were checked with mails written to disk.

License and team: see [LICENSE.md](LICENSE.md). Original team: Shiju Varghese, Sharon Sudhan, Adarsh V.S.,
Peter Kneale and contributors.
