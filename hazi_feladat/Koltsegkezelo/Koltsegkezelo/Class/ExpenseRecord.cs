using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koltsegkezelo.Class
{
    public record ExpenseRecord(DateTime Date, Category Category, string Description, decimal Amount);
}
