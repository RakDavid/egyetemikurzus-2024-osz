using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;
using Koltsegkezelo.Class;
using Koltsegkezelo.Interfaces;

namespace ExpenseTracker.Repositories
{

    public class JsonExpenseRepository : IExpenseRepository
    {
        private string GetFilePath(DateTime date)
        {
            return $"kiadasok_{date:yyyy_MM}.json";
        }

        private readonly string _categoryFilePath = "kategoriak.json";

        public List<ExpenseRecord> LoadExpenses(DateTime date)
        {
            string filePath = GetFilePath(date);
            try
            {
                if (!File.Exists(filePath)) return new List<ExpenseRecord>();
                var jsonData = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<ExpenseRecord>>(jsonData) ?? new List<ExpenseRecord>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a kiadások betöltésekor erre a hónapra: {date:yyyy-MM}: {ex.Message}");
                return new List<ExpenseRecord>();
            }
        }

        public List<DateTime> GetAvailableMonths()
        {
            var months = new List<DateTime>();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "kiadasok_*.json");

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName != null && fileName.StartsWith("kiadasok_"))
                {
                    var dateString = fileName.Replace("kiadasok_", "");
                    if (DateTime.TryParseExact(dateString, "yyyy_MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        months.Add(date);
                    }
                }
            }

            return months.OrderBy(d => d).ToList();
        }

        public void SaveExpenses(List<ExpenseRecord> expenses, DateTime date)
        {
            string filePath = GetFilePath(date);
            try
            {
                var jsonData = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a kiadások mentésekor erre a hónapra: {date:yyyy-MM}: {ex.Message}");
            }
        }

        public List<Category> LoadCategories()
        {
            try
            {
                if (!File.Exists(_categoryFilePath)) return new List<Category>();
                var jsonData = File.ReadAllText(_categoryFilePath);
                return JsonSerializer.Deserialize<List<Category>>(jsonData) ?? new List<Category>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a kategóriák betöltése közben: {ex.Message}");
                return new List<Category>();
            }
        }
        public void SaveCategories(List<Category> categories)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_categoryFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a kategóriák mentése közben: {ex.Message}");
            }
        }
    }
}

