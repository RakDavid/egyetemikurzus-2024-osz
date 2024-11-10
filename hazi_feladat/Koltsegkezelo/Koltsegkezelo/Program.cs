class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n==== Költségkezelő Menü ====");
            Console.WriteLine("1. Új költség hozzáadása");
            Console.WriteLine("2. Havi összesítő megtekintése");
            Console.WriteLine("3. Éves összesítő megtekintése");
            Console.WriteLine("4. Kilépés");
            Console.WriteLine("=============================");

            string choice = Console.ReadLine();
            if (choice == "4") break;
            Console.WriteLine("Ez a funkció jelenleg nem elérhető.");
        }
    }
}