using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Koltsegkezelo.Class;
using Koltsegkezelo.Interfaces;

namespace Koltsegkezelo.UI
{
    public class ConsoleUserInterface : IUserInterface
    {
        public void DisplayMessage(string message) => Console.WriteLine(message);

        public string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        public void DisplayExpenses(List<ExpenseRecord> expenses)
        {
            foreach (var expense in expenses)
                Console.WriteLine(expense);
        }
    }
}
