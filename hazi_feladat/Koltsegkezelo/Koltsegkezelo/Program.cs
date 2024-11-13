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

    
}