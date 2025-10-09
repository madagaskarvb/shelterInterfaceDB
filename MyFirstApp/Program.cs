using System;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Interfaces;
using AnimalShelter.Repositories;
using AnimalShelter.Services;
using AnimalShelter.Helpers;
using AnimalShelter.Models;

namespace AnimalShelter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Строка подключения к PostgreSQL
            string connectionString = "Host=138.124.14.1;Port=5432;Database=Animal House;Username=myuser;Password=mypassword";

            // Создание контекста БД
            using var context = DbContextFactory.CreateDbContext(connectionString);

            // Инициализация репозиториев
            IAnimalRepository animalRepo = new AnimalRepository(context);
            IAdopterRepository adopterRepo = new AdopterRepository(context);
            IAdoptionRepository adoptionRepo = new AdoptionRepository(context);

            // Инициализация сервисов
            var animalService = new AnimalService(animalRepo);
            var adopterService = new AdopterService(adopterRepo);
            var adoptionService = new AdoptionService(adoptionRepo, animalRepo);
            var reportService = new ReportService(context);

            bool exit = false;

            while (!exit)
            {
                ConsoleHelper.PrintHeader("СИСТЕМА УПРАВЛЕНИЯ ПРИЮТОМ ДЛЯ ЖИВОТНЫХ");
                Console.WriteLine("1. Управление животными");
                Console.WriteLine("2. Управление усыновителями");
                Console.WriteLine("3. Управление усыновлениями");
                Console.WriteLine("4. Отчеты и выборки");
                Console.WriteLine("0. Выход");
                Console.WriteLine();

                string choice = ConsoleHelper.ReadLine("Выберите пункт меню: ");

                switch (choice)
                {
                    case "1":
                        AnimalMenu(animalService);
                        break;
                    case "2":
                        AdopterMenu(adopterService);
                        break;
                    case "3":
                        AdoptionMenu(adoptionService);
                        break;
                    case "4":
                        ReportMenu(reportService);
                        break;
                    case "0":
                        exit = true;
                        ConsoleHelper.PrintSuccess("До свидания!");
                        break;
                    default:
                        ConsoleHelper.PrintError("Неверный выбор!");
                        ConsoleHelper.PressAnyKeyToContinue();
                        break;
                }
            }
        }

        static void AnimalMenu(AnimalService service)
        {
            bool back = false;
            while (!back)
            {
                ConsoleHelper.PrintHeader("УПРАВЛЕНИЕ ЖИВОТНЫМИ");
                Console.WriteLine("1. Просмотр всех животных");
                Console.WriteLine("2. Добавить животное");
                Console.WriteLine("3. Редактировать животное");
                Console.WriteLine("4. Удалить животное");
                Console.WriteLine("5. Поиск по статусу");
                Console.WriteLine("0. Назад");
                Console.WriteLine();

                string choice = ConsoleHelper.ReadLine("Выберите действие: ");

                switch (choice)
                {
                    case "1":
                        ViewAllAnimals(service);
                        break;
                    case "2":
                        AddAnimal(service);
                        break;
                    case "3":
                        UpdateAnimal(service);
                        break;
                    case "4":
                        DeleteAnimal(service);
                        break;
                    case "5":
                        SearchAnimalsByStatus(service);
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        ConsoleHelper.PrintError("Неверный выбор!");
                        ConsoleHelper.PressAnyKeyToContinue();
                        break;
                }
            }
        }

        static void ViewAllAnimals(AnimalService service)
        {
            ConsoleHelper.PrintHeader("СПИСОК ВСЕХ ЖИВОТНЫХ");
            var animals = service.GetAllAnimals();
            
            foreach (var animal in animals)
            {
                Console.WriteLine($"ID: {animal.AnimalId} | Имя: {animal.Name} | Вид: {animal.Species} | Возраст: {animal.Age} | Статус: {animal.Status}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AddAnimal(AnimalService service)
        {
            ConsoleHelper.PrintHeader("ДОБАВЛЕНИЕ НОВОГО ЖИВОТНОГО");

            var animal = new Animal
            {
                Name = ConsoleHelper.ReadLine("Имя животного: "),
                Species = ConsoleHelper.ReadLine("Вид (Dog, Cat, Bird и т.д.): "),
                Breed = ConsoleHelper.ReadLine("Порода: "),
                Age = ConsoleHelper.ReadInt("Возраст: "),
                Gender = ConsoleHelper.ReadLine("Пол (Male/Female): "),
                Description = ConsoleHelper.ReadLine("Описание: "),
                Status = "InShelter"
            };

            service.AddAnimal(animal);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void UpdateAnimal(AnimalService service)
        {
            ConsoleHelper.PrintHeader("РЕДАКТИРОВАНИЕ ЖИВОТНОГО");
            
            int id = ConsoleHelper.ReadInt("Введите ID животного: ");
            var animal = service.GetAnimalById(id);

            if (animal == null)
            {
                ConsoleHelper.PrintError("Животное не найдено!");
                ConsoleHelper.PressAnyKeyToContinue();
                return;
            }

            Console.WriteLine($"Текущее имя: {animal.Name}");
            animal.Name = ConsoleHelper.ReadLine("Новое имя (Enter - оставить прежним): ");
            
            Console.WriteLine($"Текущий возраст: {animal.Age}");
            string ageInput = ConsoleHelper.ReadLine("Новый возраст (Enter - оставить прежним): ");
            if (!string.IsNullOrWhiteSpace(ageInput))
            {
                animal.Age = int.Parse(ageInput);
            }

            service.UpdateAnimal(animal);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void DeleteAnimal(AnimalService service)
        {
            ConsoleHelper.PrintHeader("УДАЛЕНИЕ ЖИВОТНОГО");
            
            int id = ConsoleHelper.ReadInt("Введите ID животного для удаления: ");
            
            Console.Write("Вы уверены? (yes/no): ");
            if (Console.ReadLine()?.ToLower() == "yes")
            {
                service.DeleteAnimal(id);
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void SearchAnimalsByStatus(AnimalService service)
        {
            ConsoleHelper.PrintHeader("ПОИСК ЖИВОТНЫХ ПО СТАТУСУ");
            
            Console.WriteLine("Доступные статусы: InShelter, Adopted, Treatment, Quarantine");
            string status = ConsoleHelper.ReadLine("Введите статус: ");
            
            var animals = service.GetAnimalsByStatus(status);
            
            foreach (var animal in animals)
            {
                Console.WriteLine($"ID: {animal.AnimalId} | Имя: {animal.Name} | Вид: {animal.Species} | Возраст: {animal.Age}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AdopterMenu(AdopterService service)
        {
            bool back = false;
            while (!back)
            {
                ConsoleHelper.PrintHeader("УПРАВЛЕНИЕ УСЫНОВИТЕЛЯМИ");
                Console.WriteLine("1. Просмотр всех усыновителей");
                Console.WriteLine("2. Добавить усыновителя");
                Console.WriteLine("3. Редактировать усыновителя");
                Console.WriteLine("4. Удалить усыновителя");
                Console.WriteLine("0. Назад");
                Console.WriteLine();

                string choice = ConsoleHelper.ReadLine("Выберите действие: ");

                switch (choice)
                {
                    case "1":
                        ViewAllAdopters(service);
                        break;
                    case "2":
                        AddAdopter(service);
                        break;
                    case "3":
                        UpdateAdopter(service);
                        break;
                    case "4":
                        DeleteAdopter(service);
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        ConsoleHelper.PrintError("Неверный выбор!");
                        ConsoleHelper.PressAnyKeyToContinue();
                        break;
                }
            }
        }

        static void ViewAllAdopters(AdopterService service)
        {
            ConsoleHelper.PrintHeader("СПИСОК УСЫНОВИТЕЛЕЙ");
            var adopters = service.GetAllAdopters();
            
            foreach (var adopter in adopters)
            {
                Console.WriteLine($"ID: {adopter.AdopterId} | Имя: {adopter.FullName} | Email: {adopter.Email} | Телефон: {adopter.Phone}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AddAdopter(AdopterService service)
        {
            ConsoleHelper.PrintHeader("РЕГИСТРАЦИЯ УСЫНОВИТЕЛЯ");

            var adopter = new Adopter
            {
                FullName = ConsoleHelper.ReadLine("ФИО: "),
                Email = ConsoleHelper.ReadLine("Email: "),
                Phone = ConsoleHelper.ReadLine("Телефон: "),
                Address = ConsoleHelper.ReadLine("Адрес: ")
            };

            service.AddAdopter(adopter);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void UpdateAdopter(AdopterService service)
        {
            ConsoleHelper.PrintHeader("РЕДАКТИРОВАНИЕ УСЫНОВИТЕЛЯ");
            
            int id = ConsoleHelper.ReadInt("Введите ID усыновителя: ");
            var adopter = service.GetAdopterById(id);

            if (adopter == null)
            {
                ConsoleHelper.PrintError("Усыновитель не найден!");
                ConsoleHelper.PressAnyKeyToContinue();
                return;
            }

            Console.WriteLine($"Текущее имя: {adopter.FullName}");
            string newName = ConsoleHelper.ReadLine("Новое имя (Enter - оставить прежним): ");
            if (!string.IsNullOrWhiteSpace(newName))
            {
                adopter.FullName = newName;
            }

            Console.WriteLine($"Текущий телефон: {adopter.Phone}");
            string newPhone = ConsoleHelper.ReadLine("Новый телефон (Enter - оставить прежним): ");
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                adopter.Phone = newPhone;
            }

            service.UpdateAdopter(adopter);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void DeleteAdopter(AdopterService service)
        {
            ConsoleHelper.PrintHeader("УДАЛЕНИЕ УСЫНОВИТЕЛЯ");
            
            int id = ConsoleHelper.ReadInt("Введите ID усыновителя: ");
            
            Console.Write("Вы уверены? (yes/no): ");
            if (Console.ReadLine()?.ToLower() == "yes")
            {
                service.DeleteAdopter(id);
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AdoptionMenu(AdoptionService service)
        {
            bool back = false;
            while (!back)
            {
                ConsoleHelper.PrintHeader("УПРАВЛЕНИЕ УСЫНОВЛЕНИЯМИ");
                Console.WriteLine("1. Просмотр всех заявок");
                Console.WriteLine("2. Создать заявку на усыновление");
                Console.WriteLine("3. Одобрить заявку");
                Console.WriteLine("4. Отклонить заявку");
                Console.WriteLine("5. Поиск по статусу");
                Console.WriteLine("0. Назад");
                Console.WriteLine();

                string choice = ConsoleHelper.ReadLine("Выберите действие: ");

                switch (choice)
                {
                    case "1":
                        ViewAllAdoptions(service);
                        break;
                    case "2":
                        CreateAdoption(service);
                        break;
                    case "3":
                        ApproveAdoption(service);
                        break;
                    case "4":
                        RejectAdoption(service);
                        break;
                    case "5":
                        SearchAdoptionsByStatus(service);
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        ConsoleHelper.PrintError("Неверный выбор!");
                        ConsoleHelper.PressAnyKeyToContinue();
                        break;
                }
            }
        }

        static void ViewAllAdoptions(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("СПИСОК ЗАЯВОК НА УСЫНОВЛЕНИЕ");
            var adoptions = service.GetAllAdoptions();
            
            foreach (var adoption in adoptions)
            {
                Console.WriteLine($"ID: {adoption.AdoptionId} | Статус: {adoption.Status} | Дата: {adoption.AdoptionDate:dd.MM.yyyy}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void CreateAdoption(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("СОЗДАНИЕ ЗАЯВКИ НА УСЫНОВЛЕНИЕ");

            var adoption = new Adoption
            {
                AdopterId = ConsoleHelper.ReadInt("ID усыновителя: "),
                AnimalId = ConsoleHelper.ReadInt("ID животного: "),
                Notes = ConsoleHelper.ReadLine("Примечания: ")
            };

            service.CreateAdoption(adoption);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void ApproveAdoption(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("ОДОБРЕНИЕ ЗАЯВКИ");
            
            int id = ConsoleHelper.ReadInt("Введите ID заявки: ");
            service.ApproveAdoption(id);
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void RejectAdoption(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("ОТКЛОНЕНИЕ ЗАЯВКИ");
            
            int id = ConsoleHelper.ReadInt("Введите ID заявки: ");
            string reason = ConsoleHelper.ReadLine("Причина отклонения: ");
            
            service.RejectAdoption(id, reason);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void SearchAdoptionsByStatus(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("ПОИСК ЗАЯВОК ПО СТАТУСУ");
            
            Console.WriteLine("Доступные статусы: Pending, Approved, Rejected, Cancelled");
            string status = ConsoleHelper.ReadLine("Введите статус: ");
            
            var adoptions = service.GetAdoptionsByStatus(status);
            
            foreach (var adoption in adoptions)
            {
                Console.WriteLine($"ID: {adoption.AdoptionId} | Дата: {adoption.AdoptionDate:dd.MM.yyyy} | Заметки: {adoption.Notes}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void ReportMenu(ReportService service)
        {
            bool back = false;
            while (!back)
            {
                ConsoleHelper.PrintHeader("ОТЧЕТЫ И ВЫБОРКИ");
                Console.WriteLine("1. Животные по статусу");
                Console.WriteLine("2. Усыновления за период");
                Console.WriteLine("3. Животные по виду и возрасту");
                Console.WriteLine("4. Статистика приюта");
                Console.WriteLine("0. Назад");
                Console.WriteLine();

                string choice = ConsoleHelper.ReadLine("Выберите отчет: ");

                switch (choice)
                {
                    case "1":
                        AnimalsByStatusReport(service);
                        break;
                    case "2":
                        AdoptionsByDateReport(service);
                        break;
                    case "3":
                        AnimalsBySpeciesReport(service);
                        break;
                    case "4":
                        service.DisplayShelterStatistics();
                        ConsoleHelper.PressAnyKeyToContinue();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        ConsoleHelper.PrintError("Неверный выбор!");
                        ConsoleHelper.PressAnyKeyToContinue();
                        break;
                }
            }
        }

        static void AnimalsByStatusReport(ReportService service)
        {
            ConsoleHelper.PrintHeader("ОТЧЕТ: ЖИВОТНЫЕ ПО СТАТУСУ");
            
            string status = ConsoleHelper.ReadLine("Статус (InShelter/Adopted/Treatment): ");
            var animals = service.GetAnimalsByStatusReport(status);
            
            Console.WriteLine($"\nНайдено животных: {animals.Count}");
            foreach (var animal in animals)
            {
                Console.WriteLine($"{animal.Name} ({animal.Species}) - Возраст: {animal.Age}, Дата поступления: {animal.DateAdmitted:dd.MM.yyyy}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AdoptionsByDateReport(ReportService service)
        {
            ConsoleHelper.PrintHeader("ОТЧЕТ: УСЫНОВЛЕНИЯ ЗА ПЕРИОД");
            
            DateTime startDate = ConsoleHelper.ReadDate("Начальная дата (дд.мм.гггг): ");
            DateTime endDate = ConsoleHelper.ReadDate("Конечная дата (дд.мм.гггг): ");
            string status = ConsoleHelper.ReadLine("Статус (Pending/Approved/Rejected): ");
            
            var adoptions = service.GetAdoptionsByDateRangeReport(startDate, endDate, status);
            
            Console.WriteLine($"\nНайдено заявок: {adoptions.Count}");
            foreach (var adoption in adoptions)
            {
                Console.WriteLine($"Дата: {adoption.AdoptionDate:dd.MM.yyyy} | Животное: {adoption.Animal?.Name} | Усыновитель: {adoption.Adopter?.FullName}");
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AnimalsBySpeciesReport(ReportService service)
        {
            ConsoleHelper.PrintHeader("ОТЧЕТ: ЖИВОТНЫЕ ПО ВИДУ И ВОЗРАСТУ");
            
            string species = ConsoleHelper.ReadLine("Вид (Dog/Cat/Bird и т.д.): ");
            int minAge = ConsoleHelper.ReadInt("Минимальный возраст: ");
            int maxAge = ConsoleHelper.ReadInt("Максимальный возраст: ");
            
            var animals = service.GetAnimalsBySpeciesAndAgeReport(species, minAge, maxAge);
            
            Console.WriteLine($"\nНайдено животных: {animals.Count}");
            foreach (var animal in animals)
            {
                Console.WriteLine($"{animal.Name} - Возраст: {animal.Age}, Порода: {animal.Breed}");
                if (animal.MedicalRecord != null)
                {
                    Console.WriteLine($"  Мед. карта: {animal.MedicalRecord.RecordNumber}");
                }
            }
            
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
}
