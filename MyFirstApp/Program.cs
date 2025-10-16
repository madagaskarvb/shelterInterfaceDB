using System;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.UI;

namespace AnimalShelterCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                using var context = new ShelterContext();

                // Проверка подключения к БД
                if (context.Database.CanConnect())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ Подключение к базе данных установлено");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("✗ Ошибка подключения к базе данных");
                    Console.ResetColor();
                    return;
                }

                var handlers = new MenuHandlers(context);

                // Создание главного меню
                var mainMenu = new Menu("Система управления приютом для животных");

                // Меню животных
                mainMenu.AddSubMenu("Управление животными", () =>
                {
                    var animalMenu = new Menu("Управление животными");
                    animalMenu.AddItem("Список всех животных", handlers.ListAllAnimals);
                    animalMenu.AddItem("Добавить животное", handlers.AddAnimal);
                    animalMenu.AddItem("Обновить информацию", handlers.UpdateAnimal);
                    animalMenu.AddItem("Удалить животное", handlers.DeleteAnimal);
                    animalMenu.AddItem("Поиск животных", handlers.SearchAnimals);
                    animalMenu.AddItem("Детальная информация", handlers.ViewAnimalDetails);
                    return animalMenu;
                });

                // Меню усыновителей
                mainMenu.AddSubMenu("Управление усыновителями", () =>
                {
                    var adopterMenu = new Menu("Управление усыновителями");
                    adopterMenu.AddItem("Список усыновителей", handlers.ListAllAdopters);
                    adopterMenu.AddItem("Регистрация нового усыновителя", handlers.AddAdopter);
                    adopterMenu.AddItem("Обновить данные", handlers.UpdateAdopter);
                    adopterMenu.AddItem("Детальная информация", handlers.ViewAdopterDetails);
                    return adopterMenu;
                });

                // Меню усыновлений
                mainMenu.AddSubMenu("Управление усыновлениями", () =>
                {
                    var adoptionMenu = new Menu("Управление усыновлениями");
                    adoptionMenu.AddItem("Список усыновлений", handlers.ListAllAdoptions);
                    adoptionMenu.AddItem("Создать запись об усыновлении", handlers.CreateAdoption);
                    adoptionMenu.AddItem("Обновить статус", handlers.UpdateAdoption);
                    return adoptionMenu;
                });

                // Меню медицинских записей
                mainMenu.AddSubMenu("Медицинские записи", () =>
                {
                    var medicalMenu = new Menu("Медицинские записи");
                    medicalMenu.AddItem("Просмотр медицинской карты", handlers.ViewMedicalRecord);
                    medicalMenu.AddItem("Создать медицинскую карту", handlers.CreateMedicalRecord);
                    medicalMenu.AddItem("Добавить запись о вакцинации", handlers.AddVaccinationRecord);
                    medicalMenu.AddItem("Просроченные вакцинации", handlers.ViewOverdueVaccinations);
                    medicalMenu.AddItem("Справочник вакцин", handlers.ListAllVaccinations);
                    medicalMenu.AddItem("Добавить вакцину", handlers.AddVaccination);
                    return medicalMenu;
                });

                // Меню отчетов
                mainMenu.AddSubMenu("Отчеты и статистика", () =>
                {
                    var reportMenu = new Menu("Отчеты и статистика");
                    reportMenu.AddItem("Отчет по животным", handlers.GenerateAnimalReport);
                    reportMenu.AddItem("Отчет по усыновлениям", handlers.GenerateAdoptionReport);
                    return reportMenu;
                });

                // Запуск главного меню
                mainMenu.Display();

                Console.WriteLine("\nДо свидания!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine($"Детали: {ex.StackTrace}");
                Console.ResetColor();
            }
            
        }
    }
}


