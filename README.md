# 💰 Expense Tracker

A personal finance dashboard built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core**, and **Syncfusion EJ2** UI components. Track income and expenses, categorize transactions, and visualize your spending with live charts — all backed by SQL Server.

🔗 **Live demo:** [expense-tracker-three-theta.vercel.app](https://expense-tracker-three-theta.vercel.app)  Temporarily out of service

---

## ✨ Features

- **Dashboard overview** — total income, total expense, and running balance for the last 7 days at a glance
- **Interactive charts** — a doughnut chart breaking expenses down by category, and a spline chart trending income vs. expense over time (powered by Syncfusion EJ2)
- **Transaction management** — full create/edit/delete workflow for income and expense entries, with category, amount, note, and date
- **Category management** — create custom categories with an icon and a type (`Income` or `Expense`) to organize transactions
- **Recent activity** — the 5 most recent transactions surfaced directly on the dashboard
- **Server-side validation** — required fields and valid-amount checks on every form, with anti-forgery protection on all mutating requests

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| UI Components / Charts | Syncfusion EJ2 for ASP.NET Core |
| Front end | Razor Views, Bootstrap, Font Awesome |

## 📁 Project Structure

```
Expense-Tracker/
├── Controllers/
│   ├── DashboardController.cs   # Summary stats + chart data aggregation
│   ├── TransactionController.cs # Transaction CRUD
│   ├── CategoryController.cs    # Category CRUD
│   └── HomeController.cs        # Landing / error pages
├── Models/
│   ├── ApplicationDBContext.cs  # EF Core DbContext
│   ├── Transaction.cs
│   ├── Category.cs
│   └── ErrorViewModel.cs
├── Migrations/                  # EF Core migration history
├── Views/
│   ├── Dashboard/
│   ├── Transaction/
│   ├── Category/
│   └── Shared/
├── wwwroot/                      # Static assets (CSS, JS, images)
├── appsettings.json               # Configuration + connection string
└── Program.cs                    # App startup & service configuration
```

For a deeper look at how each piece fits together, see [`DOCUMENTATION.md`](DOCUMENTATION.md).

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full SQL Server)
- A free [Syncfusion community license key](https://www.syncfusion.com/products/communitylicense) (the repo ships with a placeholder key in `Program.cs` — swap in your own)

### Setup

1. **Clone the repo**
   ```bash
   git clone https://github.com/AngeloGeno/Expense-Tracker.git
   cd Expense-Tracker
   ```

2. **Configure your database connection**

   Update the `DevConnection` string in `appsettings.json` to point at your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DevConnection": "Server=YOUR_SERVER;Database=TransactionDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
   }
   ```

3. **Apply EF Core migrations**
   ```bash
   dotnet ef database update
   ```
   *(Install the tool first if needed: `dotnet tool install --global dotnet-ef`)*

4. **Run the app**
   ```bash
   dotnet run
   ```

5. Open your browser to the URL shown in the console (the app opens on the **Dashboard** by default).

## 🗺 Usage

1. Head to **Categories** and set up the categories you want to track (e.g. `🍔 Food` as an Expense, `💼 Salary` as Income).
2. Go to **Transactions** and start logging entries against those categories.
3. Return to the **Dashboard** to see your totals, category breakdown, and 7-day trend update automatically.

## 🔒 A note on the Syncfusion license key

`Program.cs` currently registers a Syncfusion license key directly in source. If you fork this project for production use, move it to configuration (`appsettings.json` or an environment variable) instead of committing it to source control.

## 🤝 Contributing

This project started as a CRUD/UI practice project. Feel free to fork it, open issues, or reach out to the author(Geno) with questions about the functionality or UI.

