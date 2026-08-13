# Expense Tracker — Technical Documentation

This document goes deeper than the README: it covers the architecture, data model, request flow, and the logic behind the dashboard so a new contributor can get productive quickly.

## 1. Architecture

The app follows the standard **ASP.NET Core MVC** pattern with a single EF Core `DbContext`:

```
Browser
   │
   ▼
Controllers (Dashboard / Transaction / Category / Home)
   │
   ▼
ApplicationDBContext  ──►  Entity Framework Core  ──►  SQL Server
   │
   ▼
Razor Views (+ Syncfusion EJ2 chart components, Bootstrap)
```

- **Routing** is convention-based, configured in `Program.cs`:
  ```csharp
  app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Dashboard}/{action=Index}/{id?}");
  ```
  The app opens on `DashboardController.Index()` by default.
- **Dependency injection**: `ApplicationDBContext` is registered with `AddDbContext` and injected into every controller via constructor injection.
- **UI components**: charts on the dashboard are rendered by Syncfusion EJ2's `ejs-accumulationchart` (doughnut) and spline chart tag helpers, fed with data the controller assembles into `ViewBag` properties.

## 2. Data Model

Two entities back the whole application, related through a foreign key.

### `Category`

| Property | Type | Notes |
|---|---|---|
| `CategoryId` | `int` | Primary key |
| `Title` | `nvarchar(50)` | Required |
| `Icon` | `nvarchar(10)` | Emoji/icon shown next to the title |
| `Type` | `nvarchar(10)` | `"Expense"` (default) or `"Income"` |
| `TitleWithIcon` | *(computed)* | Not mapped; concatenates `Icon` + `Title` for display |

### `Transaction`

| Property | Type | Notes |
|---|---|---|
| `TransactionId` | `int` | Primary key |
| `CategoryId` | `int` | Foreign key → `Category`, must be > 0 |
| `Category` | `Category` | Navigation property |
| `Amount` | `int` | Must be > 0 |
| `Note` | `nvarchar(75)` | Optional |
| `Date` | `DateTime` | Defaults to `DateTime.Now` |
| `CategoryWithIcon` | *(computed)* | Not mapped; icon + title of the linked category |
| `FormatedAmount` | *(computed)* | Not mapped; prefixes `-` for expenses, `+` for income, formatted as currency |

### Relationship

```
Category (1) ───< (many) Transaction
```

A `Category`'s `Type` determines how every `Transaction` referencing it is treated in dashboard totals — there's no separate "income/expense" flag on the transaction itself.

### `ApplicationDBContext`

Exposes two `DbSet`s: `Transactions` and `Categories`. Migrations live under `Migrations/` and are managed with standard EF Core tooling (`dotnet ef migrations add`, `dotnet ef database update`).

## 3. Controllers & Routes

| Controller | Action | Verb | Purpose |
|---|---|---|---|
| `DashboardController` | `Index` | GET | Aggregates the last 7 days of transactions into summary totals and chart data |
| `TransactionController` | `Index` | GET | Lists all transactions with their category included |
| `TransactionController` | `AddOrEdit(int id = 0)` | GET | Loads the form; `id = 0` means "new" |
| `TransactionController` | `AddOrEdit(Transaction)` | POST | Creates or updates a transaction |
| `TransactionController` | `Delete(int id)` | POST | Deletes a transaction (via `DeleteConfirmed`) |
| `CategoryController` | `Index` | GET | Lists all categories |
| `CategoryController` | `AddOrEdit(int id = 0)` | GET | Loads the form; `id = 0` means "new" |
| `CategoryController` | `AddOrEdit(Category)` | POST | Creates or updates a category |
| `CategoryController` | `Delete(int id)` | POST | Deletes a category (via `DeleteConfirmed`) |
| `HomeController` | `Index`, `Privacy`, `Error` | GET | Static/informational pages |

All POST actions that mutate data are decorated with `[ValidateAntiForgeryToken]` and use `[Bind(...)]` allow-lists to prevent over-posting.

## 4. Dashboard Logic (the interesting part)

`DashboardController.Index()` does all of its work in a single pass over the last 7 days of transactions:

1. **Window selection** — pulls every `Transaction` (with `Category` included) where `Date` falls between `Today - 6 days` and `Today`.
2. **Totals** — sums `Amount` separately for transactions whose category `Type` is `"Income"` vs `"Expense"`, then computes `Balance = Income - Expense`. Balance is formatted with a custom `CultureInfo` so negative balances render as `-$X` rather than `($X)`.
3. **Doughnut chart data** — groups the window's expense transactions by category, summing amounts per category, and sorts descending so the biggest expense category leads.
4. **Spline chart data** — builds two series (daily income, daily expense) by grouping on `Date`, then left-joins both series against a generated array of the 7 day labels (`dd-MMM`) so days with no activity still appear as zero rather than being omitted.
5. **Recent transactions** — the 5 most recent transactions (across all time, not just the 7-day window) are passed to the view for a quick-glance activity feed.

All of this is exposed to the Razor view through `ViewBag` (`TotalIncome`, `TotalExpense`, `TotalBalance`, `DoughnutChartData`, `SplineChartData`, `RecentTransactions`) rather than a strongly-typed view model — worth keeping in mind if you extend the dashboard, since there's no compile-time safety on those property names.

## 5. Configuration

| File | Purpose |
|---|---|
| `appsettings.json` | Holds the `DevConnection` SQL Server connection string and default logging config |
| `appsettings.Development.json` | Development-only overrides (currently just logging) |
| `Properties/launchSettings.json` | Local run profiles (ports, environment variables) |

The Syncfusion license key is registered in `Program.cs` via `SyncfusionLicenseProvider.RegisterLicense(...)`. Because it's hardcoded, anyone deploying this project should replace it with their own community/commercial key and, ideally, move it to configuration rather than source.

## 6. Known Limitations & Ideas for Extension

- **No authentication** — the app is single-tenant; every visitor sees the same data. Adding ASP.NET Core Identity would be the natural next step for multi-user support.
- **`ViewBag`-driven dashboard** — migrating `DashboardController.Index()` to a strongly-typed view model would make the dashboard easier to extend and test.
- **Hardcoded license key & connection string** — both belong in user secrets / environment variables for anything beyond local development.
- **Fixed 7-day window** — the dashboard only ever shows the trailing week; a date-range picker would generalize it.
- **No automated tests** — controllers currently have no unit or integration test coverage.

## 7. Local Development Checklist

- [ ] .NET 8 SDK installed
- [ ] SQL Server instance reachable, connection string updated in `appsettings.json`
- [ ] `dotnet ef database update` run to create/seed the schema
- [ ] Own Syncfusion license key in place
- [ ] `dotnet run`, then browse to the Dashboard
