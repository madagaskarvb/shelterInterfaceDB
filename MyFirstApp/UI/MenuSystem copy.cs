using System;
using System.Collections.Generic;

namespace AnimalShelterCLI.UI
{
    public class MenuItem
    {
        public string Title { get; set; }
        public Action Action { get; set; }
        public Func<Menu> SubMenu { get; set; }
    }

    public class Menu
    {
        public string Title { get; set; }
        public List<MenuItem> Items { get; set; }
        public Menu ParentMenu { get; set; }

        public Menu(string title)
        {
            Title = title;
            Items = new List<MenuItem>();
        }

        public void AddItem(string title, Action action)
        {
            Items.Add(new MenuItem { Title = title, Action = action });
        }

        public void AddSubMenu(string title, Func<Menu> subMenuFactory)
        {
            Items.Add(new MenuItem { Title = title, SubMenu = subMenuFactory });
        }

        public void Display()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"╔═══════════════════════════════════════════════════╗");
                Console.WriteLine($"║  {Title.PadRight(47)}║");
                Console.WriteLine($"╚═══════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                for (int i = 0; i < Items.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {Items[i].Title}");
                }

                if (ParentMenu != null)
                {
                    Console.WriteLine($"  0. Назад");
                }
                else
                {
                    Console.WriteLine($"  0. Выход");
                }

                Console.WriteLine();
                Console.Write("Выберите опцию: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 0)
                    {
                        return;
                    }

                    if (choice > 0 && choice <= Items.Count)
                    {
                        var selectedItem = Items[choice - 1];

                        if (selectedItem.Action != null)
                        {
                            Console.Clear();
                            selectedItem.Action.Invoke();
                            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                        }
                        else if (selectedItem.SubMenu != null)
                        {
                            var subMenu = selectedItem.SubMenu.Invoke();
                            subMenu.ParentMenu = this;
                            subMenu.Display();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор!");
                        Console.ReadKey();
                    }
                }
            }
        }
    }

    public static class MenuHelper
    {
        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n═══ {title} ═══\n");
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {message}");
            Console.ResetColor();
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
