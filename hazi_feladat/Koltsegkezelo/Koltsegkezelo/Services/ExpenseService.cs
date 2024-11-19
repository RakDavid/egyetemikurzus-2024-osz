using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Koltsegkezelo.Class;
using Koltsegkezelo.Interfaces;

namespace Koltsegkezelo.Services
{
    public class ExpenseService
    {
        private readonly IExpenseRepository _repository;
        private readonly List<Category> _categories;

        public ExpenseService(IExpenseRepository repository)
        {
            _repository = repository;
            _categories = _repository.LoadCategories();
        }

        public List<Category> GetAllCategories() => _categories;

        public void AddCategory(string name, string description = "")
        {
            if (!_categories.Any(c => c.Name == name))
            {
                var newCategory = new Category(name);
                _categories.Add(newCategory);
                _repository.SaveCategories(_categories);
            }
        }

        public Category? GetCategoryByName(string name)
        {
            return _categories.FirstOrDefault(c => c.Name == name);
        }

        public void AddExpense(List<ExpenseRecord> expenses, DateTime date, Category category, string description, decimal amount)
        {
            var expense = new ExpenseRecord(date, category, description, amount);
            expenses.Add(expense);
            _repository.SaveExpenses(expenses, date);
        }

        public IEnumerable<(string CategoryName, decimal Total)> GetExpenseSummaryByCategory(List<ExpenseRecord> expenses)
        {
            return expenses
                .GroupBy(e => e.Category.Name)
                .Select(g => (CategoryName: g.Key, Total: g.Sum(e => e.Amount)));
        }

        public (List<(DateTime Month, decimal Total)>, decimal) GetYearlySpendingSummary(int year, IExpenseRepository repository)
        {
            var monthlyTotals = new List<(DateTime Month, decimal Total)>();
            var availableMonths = repository.GetAvailableMonths();

            foreach (var month in availableMonths.Where(m => m.Year == year))
            {
                var monthlyExpenses = repository.LoadExpenses(month);
                var total = monthlyExpenses.Sum(e => e.Amount);
                monthlyTotals.Add((Month: month, Total: total));
            }

            decimal averageSpending = monthlyTotals.Any() ? monthlyTotals.Average(mt => mt.Total) : 0;

            return (monthlyTotals, averageSpending);
        }

        public (List<(string CategoryName, string Description, decimal Amount)>, Dictionary<string, decimal>) GetMonthlySummaryByCategory(List<ExpenseRecord> expenses)
        {
            var expenseDetails = expenses
                .OrderBy(e => e.Category.Name)
                .Select(e => (CategoryName: e.Category.Name, Description: e.Description, Amount: e.Amount))
                .ToList();

            var categoryTotals = expenses
                .GroupBy(e => e.Category.Name)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            return (expenseDetails, categoryTotals);
        }
    }
}
