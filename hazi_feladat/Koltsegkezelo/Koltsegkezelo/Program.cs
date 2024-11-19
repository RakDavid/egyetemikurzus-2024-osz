using ExpenseTracker.Repositories;
using Koltsegkezelo.Class;
using Koltsegkezelo.Interfaces;
using Koltsegkezelo.Services;
using Koltsegkezelo.UI;

class Program
{
    static void Main()
    {
        IExpenseRepository repository = new JsonExpenseRepository();
        ExpenseService expenseService = new ExpenseService(repository);
        IUserInterface ui = new ConsoleUserInterface();

        MainMenu(ui, expenseService, repository);
        ui.DisplayMessage("További szép napot és minél későbbi viszont látást!");
    }

    static void MainMenu(IUserInterface ui, ExpenseService service, IExpenseRepository repository)
    {
        while (true)
        {

            ui.DisplayMessage("\n==== Költségkezelő Menü ====");
            ui.DisplayMessage("1. Új költség hozzáadása");
            ui.DisplayMessage("2. Havi összesítő megtekintése");
            ui.DisplayMessage("3. Éves összesítő megtekintése");
            ui.DisplayMessage("4. Kilépés");
            ui.DisplayMessage("=============================\n");

            string choice = ui.GetUserInput("Válasszon egy lehetőséget a szám megadásával: ");
            switch (choice)
            {
                case "1":
                    AddExpense(ui, service, repository);
                    break;
                case "2":
                    ViewMonthlySummary(ui, service, repository);
                    break;
                case "3":
                    ViewYearlySummary(ui, service, repository);
                    break;
                case "4":
                    return;
                default:
                    ui.DisplayMessage("Érvénytelen választás. Adjon meg egy számot a menü lehetőségei közül.");
                    break;
            }
        }
    }

    static void AddExpense(IUserInterface ui, ExpenseService service, IExpenseRepository repository)
    {
        Console.Clear();
        ui.DisplayMessage("Költség hozzáadása. Írjon be '0'-t bármelyik lépésnél a visszatéréshez a főmenübe.");

        DateTime date;
        while (true)
        {
            string dateInput = ui.GetUserInput("Dátum (éééé-hh-nn): ");
            if (dateInput == "0") return;

            if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                break;
            }
            else
            {
                ui.DisplayMessage("Érvénytelen dátumformátum. Adjon meg egy érvényes dátumot éééé-hh-nn formátumban.");
            }
        }

        var categories = service.GetAllCategories();
        if (!categories.Any())
        {
            ui.DisplayMessage("Nincsenek elérhető kategóriák. Egy alapértelmezett kategória hozzáadása.");
            service.AddCategory("Nincs kategorizálva");
            categories = service.GetAllCategories();
        }

        ui.DisplayMessage("Válasszon egy kategóriát vagy hozzon létre újat:");
        for (int i = 0; i < categories.Count; i++)
        {
            ui.DisplayMessage($"{i + 1}. {categories[i].Name}");
        }
        ui.DisplayMessage($"{categories.Count + 1}. Új kategória hozzáadása");
        ui.DisplayMessage("0. Vissza a főmenübe");

        int categoryIndex;
        while (true)
        {
            string categoryInput = ui.GetUserInput("Adja meg a kategória számát, vagy '0' a visszalépéshez: ");
            if (categoryInput == "0") return;

            if (int.TryParse(categoryInput, out categoryIndex))
            {
                if (categoryIndex == categories.Count + 1)
                {
                    string categoryName = ui.GetUserInput("Új kategória neve: ");
                    if (categoryName == "0") return;

                    service.AddCategory(categoryName);
                    categories = service.GetAllCategories();
                    categoryIndex = categories.Count - 1;
                    break;
                }
                else if (categoryIndex > 0 && categoryIndex <= categories.Count)
                {
                    categoryIndex--;
                    break;
                }
                else
                {
                    ui.DisplayMessage("Érvénytelen kategória választás. Próbálja újra.");
                }
            }
            else
            {
                ui.DisplayMessage("Érvénytelen bevitel. Adjon meg egy számot.");
            }
        }

        Category selectedCategory = categories[categoryIndex];

        string description = ui.GetUserInput("Kiadás részletes leírása: ");
        if (description == "0") return;

        decimal amount;
        while (true)
        {
            string amountInput = ui.GetUserInput("Összeg: ");
            if (amountInput == "0") return;

            if (decimal.TryParse(amountInput, out amount) && amount > 0)
            {
                break;
            }
            else
            {
                ui.DisplayMessage("Érvénytelen összeg. Adjon meg egy pozitív számot.");
            }
        }

        List<ExpenseRecord> monthlyExpenses = repository.LoadExpenses(date);
        service.AddExpense(monthlyExpenses, date, selectedCategory, description, amount);

        ui.DisplayMessage($"Költség hozzáadva erre az időpontra: {date:yyyy-MM-dd}");
    }

    static void ViewMonthlySummary(IUserInterface ui, ExpenseService service, IExpenseRepository repository)
    {
        Console.Clear();
        var availableMonths = repository.GetAvailableMonths();
        if (!availableMonths.Any())
        {
            ui.DisplayMessage("Nincsenek elérhető hónapok költségadatokkal.");
            return;
        }

        ui.DisplayMessage("Elérhető hónapok az összegzéshez:");
        for (int i = 0; i < availableMonths.Count; i++)
        {
            ui.DisplayMessage($"{i + 1}. {availableMonths[i]:yyyy-MM}");
        }

        int monthIndex = int.Parse(ui.GetUserInput("Adja meg az összegzendő hónap számát: ")) - 1;
        if (monthIndex < 0 || monthIndex >= availableMonths.Count)
        {
            ui.DisplayMessage("Érvénytelen választás.");
            return;
        }

        DateTime selectedMonth = availableMonths[monthIndex];

        List<ExpenseRecord> monthlyExpenses = repository.LoadExpenses(selectedMonth);
        if (monthlyExpenses.Count == 0)
        {
            ui.DisplayMessage("Nincsenek költségek a megadott hónaphoz: " + selectedMonth.ToString("yyyy-MM"));
            return;
        }

        var (expenseDetails, categoryTotals) = service.GetMonthlySummaryByCategory(monthlyExpenses);

        ui.DisplayMessage("\nKöltségek összegzése:");
        foreach (var item in expenseDetails)
        {
            ui.DisplayMessage($"Kategória: {item.CategoryName}, Leírás: {item.Description}, Összeg: {item.Amount:C}");
        }

        ui.DisplayMessage("\nKategóriánkénti összesített költés:");
        foreach (var category in categoryTotals)
        {
            ui.DisplayMessage($"{category.Key}: {category.Value:C}");
        }

        decimal totalSpending = monthlyExpenses.Sum(e => e.Amount);
        ui.DisplayMessage($"\nTeljes költés: {totalSpending:C}");
    }

    static void ViewYearlySummary(IUserInterface ui, ExpenseService service, IExpenseRepository repository)
    {
        Console.Clear();
        var availableYears = repository.GetAvailableMonths()
                                       .Select(m => m.Year)
                                       .Distinct()
                                       .OrderBy(y => y)
                                       .ToList();

        if (!availableYears.Any())
        {
            ui.DisplayMessage("Nincsenek elérhető évek költségadatokkal.");
            return;
        }

        ui.DisplayMessage("Elérhető évek az összegzéshez:");
        for (int i = 0; i < availableYears.Count; i++)
        {
            ui.DisplayMessage($"{i + 1}. {availableYears[i]}");
        }

        int yearIndex = int.Parse(ui.GetUserInput("Adja meg az összegzendő év számát: ")) - 1;
        if (yearIndex < 0 || yearIndex >= availableYears.Count)
        {
            ui.DisplayMessage("Érvénytelen választás.");
            return;
        }

        int selectedYear = availableYears[yearIndex];

        var (monthlyTotals, averageSpending) = service.GetYearlySpendingSummary(selectedYear, repository);

        ui.DisplayMessage($"\nÉves költési összegzés ehhez az évhez {selectedYear}:");
        foreach (var (Month, Total) in monthlyTotals)
        {
            ui.DisplayMessage($"{Month:MMMM}: {Total:C}");
        }

        ui.DisplayMessage($"\nHavi átlagos költés: {averageSpending:C}");
    }


}