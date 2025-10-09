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
                Console.WriteLine($"ID: {animal.AnimalId} | Имя: {animal.Name} | Вид: {animal.Species} | Возраст: {animal.Age} | Статус: {animal.AnimalStatus?.StatusName}");
            }
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void AddAnimal(AnimalService service)
        {
            ConsoleHelper.PrintHeader("ДОБАВЛЕНИЕ НОВОГО ЖИВОТНОГО");
            Console.WriteLine("Доступные статусы:");
            Console.WriteLine("1 - InShelter (В приюте)");
            Console.WriteLine("2 - Adopted (Усыновлен)");
            Console.WriteLine("3 - Treatment (Лечение)");
            Console.WriteLine("4 - Quarantine (Карантин)");
            
            string name = ConsoleHelper.ReadLine("Имя животного: ");
            int age = ConsoleHelper.ReadInt("Возраст: ");
            string species = ConsoleHelper.ReadLine("Вид (Dog, Cat, Bird и т.д.): ");
            string breed = ConsoleHelper.ReadLine("Порода: ");
            string gender = ConsoleHelper.ReadLine("Пол (Male/Female): ");
            int statusId = ConsoleHelper.ReadInt("ID статуса (1-4): ");
            string description = ConsoleHelper.ReadLine("Описание: ");

            service.AddAnimal(name, age, species, breed, gender, statusId, description);
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
            string name = ConsoleHelper.ReadLine("Новое имя (Enter - оставить прежним): ");

            Console.WriteLine($"Текущий возраст: {animal.Age}");
            string ageInput = ConsoleHelper.ReadLine("Новый возраст (Enter - оставить прежним): ");
            int? age = string.IsNullOrWhiteSpace(ageInput) ? null : int.Parse(ageInput);

            Console.WriteLine($"Текущий статус: {animal.AnimalStatus?.StatusName}");
            string statusInput = ConsoleHelper.ReadLine("Новый ID статуса (Enter - оставить прежним): ");
            int? statusId = string.IsNullOrWhiteSpace(statusInput) ? null : int.Parse(statusInput);

            service.UpdateAnimal(id, name, age, null, null, null, statusId, null);
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
            Console.WriteLine("Доступные статусы:");
            Console.WriteLine("1 - InShelter");
            Console.WriteLine("2 - Adopted");
            Console.WriteLine("3 - Treatment");
            Console.WriteLine("4 - Quarantine");
            
            int statusId = ConsoleHelper.ReadInt("Введите ID статуса: ");
            var animals = service.GetAnimalsByStatus(statusId);
            
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
            string firstName = ConsoleHelper.ReadLine("Имя: ");
            string lastName = ConsoleHelper.ReadLine("Фамилия: ");
            string email = ConsoleHelper.ReadLine("Email: ");
            string phone = ConsoleHelper.ReadLine("Телефон: ");
            string address = ConsoleHelper.ReadLine("Адрес: ");

            service.RegisterAdopter(firstName, lastName, email, phone, address);
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
            string firstName = ConsoleHelper.ReadLine("Новое имя (Enter - оставить прежним): ");
            string lastName = ConsoleHelper.ReadLine("Новая фамилия (Enter - оставить прежним): ");

            Console.WriteLine($"Текущий телефон: {adopter.Phone}");
            string phone = ConsoleHelper.ReadLine("Новый телефон (Enter - оставить прежним): ");

            service.UpdateAdopter(id, firstName, lastName, null, phone, null);
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
                Console.WriteLine("3. Обновить статус заявки");
                Console.WriteLine("4. Вернуть животное");
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
                        UpdateAdoptionStatus(service);
                        break;
                    case "4":
                        ReturnAnimal(service);
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
                Console.WriteLine($"ID: {adoption.AdoptionId} | Статус: {adoption.AdoptionStatus?.StatusName} | Дата: {adoption.AdoptionDate:dd.MM.yyyy}");
            }
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void CreateAdoption(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("СОЗДАНИЕ ЗАЯВКИ НА УСЫНОВЛЕНИЕ");
            Console.WriteLine("Доступные статусы:");
            Console.WriteLine("1 - Pending (Ожидает)");
            Console.WriteLine("2 - Approved (Одобрено)");
            Console.WriteLine("3 - Rejected (Отклонено)");
            Console.WriteLine("4 - Cancelled (Отменено)");
            
            int adopterId = ConsoleHelper.ReadInt("ID усыновителя: ");
            int animalId = ConsoleHelper.ReadInt("ID животного: ");
            int statusId = ConsoleHelper.ReadInt("ID статуса (обычно 1 - Pending): ");
            string notes = ConsoleHelper.ReadLine("Примечания: ");

            service.CreateAdoption(adopterId, animalId, statusId, notes);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void UpdateAdoptionStatus(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("ОБНОВЛЕНИЕ СТАТУСА ЗАЯВКИ");
            int id = ConsoleHelper.ReadInt("Введите ID заявки: ");
            Console.WriteLine("Доступные статусы:");
            Console.WriteLine("1 - Pending");
            Console.WriteLine("2 - Approved");
            Console.WriteLine("3 - Rejected");
            Console.WriteLine("4 - Cancelled");
            
            int newStatusId = ConsoleHelper.ReadInt("Новый ID статуса: ");
            service.UpdateAdoptionStatus(id, newStatusId);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void ReturnAnimal(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("ВОЗВРАТ ЖИВОТНОГО");
            int id = ConsoleHelper.ReadInt("Введите ID усыновления: ");
            DateTime returnDate = ConsoleHelper.ReadDate("Дата возврата (дд.мм.гггг): ");
            service.ReturnAnimal(id, returnDate);
            ConsoleHelper.PressAnyKeyToContinue();
        }

        static void SearchAdoptionsByStatus(AdoptionService service)
        {
            ConsoleHelper.PrintHeader("ПОИСК ЗАЯВОК ПО СТАТУСУ");
            Console.WriteLine("Доступные статусы:");
            Console.WriteLine("1 - Pending");
            Console.WriteLine("2 - Approved");
            Console.WriteLine("3 - Rejected");
            Console.WriteLine("4 - Cancelled");
            
            int statusId = ConsoleHelper.ReadInt("Введите ID статуса: ");
            var adoptions = service.GetAdoptionsByStatus(statusId);
            
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
                Console.WriteLine("1. Животные");
                Console.WriteLine("2. Усыновители");
                Console.WriteLine("3. Заявки на усыновление");
                Console.WriteLine("4. Отчеты");
                Console.WriteLine("0. Выход");
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
            int statusId = ConsoleHelper.ReadInt("ID статуса (1-4): ");
            var animals = service.GetAnimalsByStatusReport(statusId);
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
            int statusId = ConsoleHelper.ReadInt("ID статуса (1-4): ");
            
            var adoptions = service.GetAdoptionsByDateRangeReport(startDate, endDate, statusId);
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
