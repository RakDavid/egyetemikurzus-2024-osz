using Koltsegkezelo.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koltsegkezelo.Interfaces
{
    public interface IExpenseRepository
    {
        List<ExpenseRecord> LoadExpenses(DateTime date);
        void SaveExpenses(List<ExpenseRecord> expenses, DateTime date);

        List<Category> LoadCategories();
        void SaveCategories(List<Category> categories);

        List<DateTime> GetAvailableMonths();
    }
}
