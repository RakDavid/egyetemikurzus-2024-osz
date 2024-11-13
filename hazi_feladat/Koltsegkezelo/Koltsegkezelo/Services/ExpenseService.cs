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

    }
}
