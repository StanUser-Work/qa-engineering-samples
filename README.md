# QA Engineering Samples

[![tests](https://github.com/StanUser-Work/qa-engineering-samples/actions/workflows/tests.yml/badge.svg)](https://github.com/StanUser-Work/qa-engineering-samples/actions/workflows/tests.yml)

This repo has two parts:

- **4 automated tests** in C# / .NET 10: 2 UI tests (Playwright) and 2 API tests (HttpClient). Each test checks expected results.
- **A manual QA example**: a bug report and test cases for my pet project, an appointment booking website.

| Part | Target | Tools |
|---|---|---|
| UI tests | [Sauce Demo](https://www.saucedemo.com), a public demo shop made for test automation | NUnit, Microsoft Playwright, Page Object pattern |
| API tests | [Restful-Booker](https://restful-booker.herokuapp.com/apidoc), a public demo REST API for hotel bookings | NUnit, `HttpClient`, `System.Text.Json` |
| Manual QA | Booking Website, my pet project (Flask + PostgreSQL, runs locally in Docker) | Chrome, Mailpit, admin dashboard |

**Quick start** (if you already have the .NET 10 SDK and PowerShell):

```bash
git clone https://github.com/StanUser-Work/qa-engineering-samples.git
cd qa-engineering-samples
dotnet build
pwsh tests/UiTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test
```

On Windows without PowerShell 7, replace the 4th line with
`powershell -ExecutionPolicy Bypass -File tests/UiTests/bin/Debug/net10.0/playwright.ps1 install chromium`.

Otherwise, follow the step-by-step guide below.

---

## Automated tests

| # | Type | Test | What it checks |
|---|---|---|---|
| 1 | UI | [`ValidUser_LogsIn_AndSeesProductCatalog`](tests/UiTests/Tests/LoginTests.cs#L10) | Valid login opens the catalog with 6 products |
| 2 | UI | [`AddProductToCart_ShowsProductInCart_WithCorrectNameAndPrice`](tests/UiTests/Tests/CartTests.cs#L22) | An added product is in the cart with the right name, price and quantity |
| 3 | API | [`CreateBooking_ThenGetById_ReturnsSameData`](tests/ApiTests/Tests/BookingApiTests.cs#L42) | A created booking is stored exactly as sent |
| 4 | API | [`DeleteBooking_WithValidToken_RemovesBooking`](tests/ApiTests/Tests/BookingApiTests.cs#L68) | An authorized delete removes the booking (then `GET` returns 404) |

Every step and expected result is listed under **How the tests work** below.

### Code structure

| Path | Contents |
|---|---|
| [`tests/UiTests/UiTestBase.cs`](tests/UiTests/UiTestBase.cs) | Shared setup: base URL, viewport, and a screenshot saved on failure |
| [`tests/UiTests/Pages/`](tests/UiTests/Pages/) | Page Objects: [`LoginPage`](tests/UiTests/Pages/LoginPage.cs), [`InventoryPage`](tests/UiTests/Pages/InventoryPage.cs), [`CartPage`](tests/UiTests/Pages/CartPage.cs) |
| [`tests/UiTests/Tests/`](tests/UiTests/Tests/) | UI tests |
| [`tests/ApiTests/Client/BookingApiClient.cs`](tests/ApiTests/Client/BookingApiClient.cs) | Small API client over `HttpClient` |
| [`tests/ApiTests/Models/Booking.cs`](tests/ApiTests/Models/Booking.cs) | Request/response models as C# records, so whole objects are compared in one assertion |
| [`tests/ApiTests/Tests/`](tests/ApiTests/Tests/) | API tests |
| [`.github/workflows/tests.yml`](.github/workflows/tests.yml) | GitHub Actions runs all tests on every push |

### How the tests work

<details>
<summary><b>Click to expand: shared setup, the steps of each test, and how I checked the tests can fail</b></summary>

&nbsp;

**Shared setup**

- **UI:** every test gets a fresh browser context (no shared cookies), so tests are independent and can run in any order.
  Page Objects hold the locators and actions, and the tests contain only the steps and the checks.
  Playwright's `Expect(...)` retries until the condition is met (up to 5 s), so slow pages don't cause false failures.
  On failure, a full-page screenshot is attached to the result.
- **API:** requests go through `BookingApiClient`, which returns the raw response, so the tests check status codes,
  headers and bodies themselves. Each booking a test creates is deleted in `[TearDown]`, even if the test fails.

**1. `ValidUser_LogsIn_AndSeesProductCatalog`**
1. Open the login page and log in as `standard_user`.
2. Check the URL is `/inventory.html`, the title is "Products", 6 products are listed, and no error is shown.

**2. `AddProductToCart_ShowsProductInCart_WithCorrectNameAndPrice`**
1. `[SetUp]`: log in.
2. Check the starting state: the cart badge is hidden (empty cart), and the backpack costs $29.99.
3. Click **Add to cart** on "Sauce Labs Backpack". Check the badge shows "1" and the button now says "Remove".
4. Open the cart. Check the URL is `/cart.html` and the title is "Your Cart". Check there is exactly one item, with the same name, price $29.99 and quantity 1.

**3. `CreateBooking_ThenGetById_ReturnsSameData`**
1. Build a booking with a unique last name.
2. `POST /booking`: check 200, an id greater than 0, and that the response echoes the sent booking.
3. `GET /booking/{id}`: check 200, the `application/json` content type, and that the stored booking equals the sent one.
   The models are C# records, so a single equality check compares every field, including the nested dates.

**4. `DeleteBooking_WithValidToken_RemovesBooking`**
1. Create a booking (precondition, checked for 200).
2. `POST /auth` with the demo admin login: check 200 and that a token was issued.
3. `DELETE /booking/{id}` with the token in a cookie: check 201 (Restful-Booker's documented success code for delete).
4. `GET /booking/{id}`: check 404. This confirms the booking is actually gone, not just that the delete returned OK.

**Do the tests catch failures?** I checked by changing one expected value in each project (the price to $19.99,
and the expected status after delete to 200). Both tests failed with clear messages, and the UI test saved a screenshot.
After I restored the values, all tests passed.

</details>

---

## Step-by-step guide for testers

### What you need

| Requirement | Why | How to check |
|---|---|---|
| **.NET 10 SDK** (any 10.0.x). The runtime alone is not enough | Builds and runs the tests | `dotnet --list-sdks` shows a line starting with `10.` |
| **PowerShell** | Used once, to download the test browser | Windows: built in. macOS/Linux: `pwsh --version` |
| **Internet access** to `www.saucedemo.com` and `restful-booker.herokuapp.com` | The tests run against these public sites | Open both links in a browser |
| **Git** (optional) | To clone the repo. Without Git, use **Code → Download ZIP** on GitHub and unzip it | `git --version` |
| About **500 MB** of free disk space | .NET packages and the Chromium browser | |

You don't need anything else. There are no accounts to create, no API keys, no config files or environment
variables, no database and no Docker.

### 1. Install the requirements

**.NET 10 SDK:** download it from https://dotnet.microsoft.com/download/dotnet/10.0 (the **SDK** column, not "Runtime"), or:

| OS | Command |
|---|---|
| Windows | `winget install Microsoft.DotNet.SDK.10` |
| macOS | `brew install --cask dotnet-sdk` |
| Linux | see https://learn.microsoft.com/dotnet/core/install/linux (instructions for each distribution) |

**PowerShell** (only on macOS/Linux; on Windows use the built-in Windows PowerShell):

| OS | Command |
|---|---|
| macOS | `brew install --cask powershell` |
| Linux | see https://learn.microsoft.com/powershell/scripting/install/installing-powershell-on-linux |

Open a **new** terminal after installing, so the new commands are found.

### 2. Get the code and build it

```bash
git clone https://github.com/StanUser-Work/qa-engineering-samples.git
cd qa-engineering-samples
dotnet build
```

The first build downloads NuGet packages and takes about a minute. It must end with `Build succeeded`.
Run all of the following commands from this `qa-engineering-samples` folder.

### 3. Install the test browser (once per machine)

The UI tests use their own copy of Chromium. It doesn't touch your regular browser. Run the command for your terminal:

| Terminal | Command |
|---|---|
| Windows PowerShell or cmd | `powershell -ExecutionPolicy Bypass -File tests/UiTests/bin/Debug/net10.0/playwright.ps1 install chromium` |
| macOS | `pwsh tests/UiTests/bin/Debug/net10.0/playwright.ps1 install chromium` |
| Linux | `pwsh tests/UiTests/bin/Debug/net10.0/playwright.ps1 install --with-deps chromium` (may ask for your sudo password to install system libraries) |

`-ExecutionPolicy Bypass` applies only to this one command. It doesn't change any settings on your machine.

### 4. Run the tests

```bash
dotnet test
```

All 4 tests take about 15 seconds. The browser runs hidden (headless). Expected output:

```
Passed!  - Failed: 0, Passed: 2, Skipped: 0, Total: 2 - ApiTests.dll (net10.0)
Passed!  - Failed: 0, Passed: 2, Skipped: 0, Total: 2 - UiTests.dll (net10.0)
```

The two lines may appear in either order.

### More ways to run

| What | Command |
|---|---|
| Only API tests | `dotnet test --filter Category=API` |
| Only UI tests | `dotnet test --filter Category=UI` |
| One test by name | `dotnet test --filter "Name~AddProductToCart"` |
| Watch the browser while UI tests run (slowed down to half a second per action) | `dotnet test --filter Category=UI -- Playwright.LaunchOptions.Headless=false Playwright.LaunchOptions.SlowMo=500` |
| Show each test's name and result | `dotnet test --logger "console;verbosity=detailed"` |
| Save a results file | `dotnet test --logger trx --results-directory TestResults` (open the `.trx` file in Visual Studio, or read it as XML) |

**In an IDE:** open `QaAutomation.slnx` in Visual Studio 2026, a recent JetBrains Rider, or VS Code with the
C# Dev Kit extension (Visual Studio 2022 can't build .NET 10 projects), and run the tests from the Test Explorer. Do steps 2 and 3 first so the browser is installed.

### What the tests do with data

- **UI tests** log in to Sauce Demo with `standard_user` / `secret_sauce`. These are public demo credentials, shown on
  the Sauce Demo login page. Every test gets a fresh browser session, so tests don't affect each other and can
  run in any order.
- **API tests** use Restful-Booker's public demo login `admin` / `password123` (published in its docs).
  Each test creates its own booking with a unique last name (`Auto-xxxxxxxx`) and deletes it at the end.
  Other people use the same demo API, and it resets its data from time to time. The tests don't depend on any
  existing data, so this doesn't affect them.
- No tests write anything to your machine except build output in `bin/` and `obj/`.

### When a test fails

1. **Read the message.** Every check states what was expected and what was received, for example
   `Expected: OK  But was: NotFound`, or `Locator expected to have text '$29.99'`.
2. **UI test:** a full-page screenshot from the moment of failure is saved to
   `tests/UiTests/bin/Debug/net10.0/screenshots/<test name>.png`.
3. **Run it again with the browser visible** (see "Watch the browser" in the table above) to see what happens.
4. **Check whether the site is up.** Open https://www.saucedemo.com or https://restful-booker.herokuapp.com/ping
   (the second should say `Created`). If a site is down, its tests fail for reasons outside this project. Try again later.
5. **On GitHub:** open the **Actions** tab, choose the run, and download the `test-results` artifact. It contains
   the results files and any failure screenshots.

### Troubleshooting

<details>
<summary><b>Click to expand: common errors and how to fix them</b></summary>

&nbsp;

| Problem | Fix |
|---|---|
| `A compatible .NET SDK was not found` or `Requested SDK version: 10.0.100` | Install the .NET 10 **SDK** (step 1) and open a new terminal. `dotnet --list-sdks` must show `10.x` |
| `Executable doesn't exist at ...ms-playwright...` or `Please run the following command to download new browsers` | The test browser is not installed. Do step 3 |
| `playwright.ps1` not found | Run `dotnet build` first (step 2). The script appears after the build |
| `pwsh: command not found` | On Windows, use the `powershell` command from step 3. On macOS/Linux, install PowerShell (step 1) |
| `running scripts is disabled on this system` | Use the exact Windows command from step 3, including `-ExecutionPolicy Bypass` |
| Linux: `Host system is missing dependencies` | Run the Linux command from step 3, with `--with-deps` |
| Timeouts or `No such host is known` | Check your internet connection, VPN or proxy. Both sites must open in a normal browser |
| An API test gets `418 I'm a teapot` | This happens only if the `Accept: application/json` header was removed from `BookingApiClient`. It's an API quirk |
| `DELETE` returning 201 instead of 200/204 looks like a bug | It's Restful-Booker's documented behavior, so the test expects 201 |

</details>

---

## Manual QA

Both documents are based on my pet project, an appointment booking website. The client picks a service and a free time,
and the administrator confirms the request in a dashboard.

> **Note:** the pet project's source code is private on purpose. The manual QA documents were written black-box,
> from the user's and administrator's side, and contain no code from the project. They are meant to be read, not
> re-run. Each document states the environment and test data used.

| Document | Contents |
|---|---|
| [Bug report BUG-001](manual-qa/bug-report.md) | A real bug found and reproduced: a second booking with the same email overwrites the name and phone on the client's earlier appointment. Includes steps, expected vs actual result, severity and a screenshot |
| [Test cases](manual-qa/test-cases.md) | 5 test cases for the booking form (positive and negative). TC-05 found BUG-001 |
