using Koltsegkezelo.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koltsegkezelo.Interfaces
{
    public interface IUserInterface
    {
        void DisplayMessage(string message);
        string GetUserInput(string prompt);
        void DisplayExpenses(List<ExpenseRecord> expenses);
    }
}
