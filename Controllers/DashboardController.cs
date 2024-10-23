using Expense_Tracker.Migrations;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _context;

        public DashboardController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            //Last 7 days
            DateTime StarDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StarDate && y.Date <= EndDate).ToListAsync();

            //Total income 
            int TotalIncome = SelectedTransactions.Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");


            //total expense 
            int TotalExpense = SelectedTransactions.Where(i => i.Category.Type == "Expense")
             .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //total balance
            int TotalBalance = TotalIncome - TotalExpense;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.TotalBalance = String.Format(culture, "{0:C0}", TotalBalance);

            //Doughnut chart data 
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId).Select(k =>
                new {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formatedAmount = k.Sum(j => j.Amount).ToString("C0"),

                }
                ).OrderByDescending(q=> q.amount).ToList();

            //SplineCahrt days
            List<SplineChartData> IncomeSummamry = SelectedTransactions.Where(m => m.Category.Type == "Income")
                 .GroupBy(h => h.Date)
                 .Select(j => new SplineChartData()
                 {
                     day = j.First().Date.ToString("dd-MMM"),
                     income =j.Sum(j => j.Amount)
                 }).ToList();
            //SplineCahrt Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions.Where(m => m.Category.Type == "Expense")
                 .GroupBy(h => h.Date)
                 .Select(j => new SplineChartData()
                 {
                     day = j.First().Date.ToString("dd-MMM"),
                     expense = j.Sum(j => j.Amount)
                 }).ToList();

            //Combined Income and Expense 
            string[] Last7Days = Enumerable.Range(0, 7)
               .Select(i => StarDate.AddDays(i).ToString("dd-MMM"))
               .ToArray();


            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummamry on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            //Recent Transactions
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category).OrderByDescending(j => j.Date)
                .Take(5).ToListAsync();



            return View();

        }
        public class SplineChartData
        {
            public string day;
            public int income;
            public int expense;

        }


    }
}
