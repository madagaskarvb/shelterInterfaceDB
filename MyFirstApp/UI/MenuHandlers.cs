using System;
using System.Linq;
using System.Collections.Generic;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;
using AnimalShelterCLI.Services;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterCLI.UI
{
    public class MenuHandlers
    {
        private readonly AnimalService _animalService;
        private readonly AdopterService _adopterService;
        private readonly AdoptionService _adoptionService;
        private readonly MedicalService _medicalService;

        public MenuHandlers(ShelterContext context)
        {
            _animalService = new AnimalService(context);
            _adopterService = new AdopterService(context);
            _adoptionService = new AdoptionService(context);
            _medicalService = new MedicalService(context);
        }

        #region Helpers

        private static string NormalizeMedicalRecordNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Убираем пробелы по краям и переводим в верхний регистр
            var normalized = input.Trim().ToUpperInvariant();

            // Если начинается с "MR" и нет дефиса после MR, добавляем
            if (normalized.StartsWith("MR") && normalized.Length > 2)
            {
                // Проверяем, что после MR не дефис
                if (normalized[2] != '-')
                {
                    // Вставляем дефис после MR
                    normalized = "MR-" + normalized.Substring(2);
                }
            }
            // Если пользователь ввёл только цифры (например, "0001"), добавляем MR- в начало
            else if (normalized.All(char.IsDigit))
            {
                normalized = "MR-" + normalized;
            }

            return normalized;
        }

        private static string ReadNonEmpty(string prompt)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            return s ?? string.Empty;
        }

        private static string ReadOptional(string prompt, string current = "")
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? (current ?? string.Empty) : s;
        }

        private static int? ReadIntOptional(string prompt, int? current = null)
        {
            Console.Write(prompt);
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text)) return current;
            if (int.TryParse(text, out var val)) return val;
            return current;
        }

        private static int ReadIntRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var text = Console.ReadLine();
                if (int.TryParse(text, out var val))
                    return val;

                MenuHelper.PrintError("Введите корректное целое число.");
            }
        }

        private static DateTime? ReadDateOptional(string prompt, DateTime? current = null)
        {
            Console.Write(prompt);
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text)) return current;
            if (DateTime.TryParse(text, out var dt)) return dt;
            return current;
        }

        private static bool Confirm(string prompt)
        {
            Console.Write($"{prompt} (y/n): ");
            var ans = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
            return ans == "y" || ans == "yes" || ans == "д" || ans == "да";
        }

        private static string ChooseGender(string current = "Unknown")
        {
            Console.WriteLine("\nВыберите пол:");
            Console.WriteLine("1. Male (Мужской)");
            Console.WriteLine("2. Female (Женский)");
            Console.WriteLine("3. Unknown (Неизвестно)");
            Console.Write("Ваш выбор (Enter — оставить без изменений): ");
            var choice = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(choice)) return string.IsNullOrWhiteSpace(current) ? "Unknown" : current;

            return choice switch
            {
                "1" => "Male",
                "2" => "Female",
                "3" => "Unknown",
                _ => string.IsNullOrWhiteSpace(current) ? "Unknown" : current
            };
        }

        private static T? ChooseFromList<T>(IList<T> items, Func<T, string> render, string title, string emptyMessage)
        {
            MenuHelper.PrintHeader(title);
            if (items == null || items.Count == 0)
            {
                MenuHelper.PrintInfo(emptyMessage);
                return default;
            }

            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {render(items[i])}");
            }

            Console.Write("Выберите номер: ");
            if (!int.TryParse(Console.ReadLine(), out var idx) || idx < 1 || idx > items.Count)
            {
                MenuHelper.PrintError("Неверный выбор.");
                return default;
            }

            return items[idx - 1];
        }

        private AnimalStatus? ChooseAnimalStatus(int? currentStatusId = null)
        {
            var statuses = _animalService.GetAllStatuses();
            if (!statuses.Any())
            {
                MenuHelper.PrintError("В базе нет статусов животных. Сначала добавьте статусы.");
                return null;
            }

            Console.WriteLine("\nДоступные статусы:");
            foreach (var s in statuses)
            {
                var mark = (currentStatusId.HasValue && s.StatusId == currentStatusId.Value) ? " (текущий)" : "";
                Console.WriteLine($"{s.StatusId}. {s.StatusName}{mark}");
            }
            Console.Write("Выберите ID статуса (Enter — без изменений): ");
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text))
                return statuses.FirstOrDefault(x => x.StatusId == currentStatusId);

            if (!int.TryParse(text, out var statusId))
            {
                MenuHelper.PrintError("Неверный ID статуса.");
                return null;
            }

            var status = statuses.FirstOrDefault(x => x.StatusId == statusId);
            if (status == null) MenuHelper.PrintError("Статус не найден.");
            return status;
        }

        private AdoptionStatus? ChooseAdoptionStatus(int? currentStatusId = null)
        {
            var statuses = _adoptionService.GetAllStatuses();
            if (!statuses.Any())
            {
                MenuHelper.PrintError("В базе нет статусов усыновления.");
                return null;
            }

            Console.WriteLine("\nДоступные статусы усыновления:");
            foreach (var s in statuses)
            {
                var mark = (currentStatusId.HasValue && s.StatusId == currentStatusId.Value) ? " (текущий)" : "";
                Console.WriteLine($"{s.StatusId}. {s.StatusName}{mark}");
            }
            Console.Write("Выберите ID статуса (Enter — без изменений): ");
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text))
                return statuses.FirstOrDefault(x => x.StatusId == currentStatusId);

            if (!int.TryParse(text, out var statusId))
            {
                MenuHelper.PrintError("Неверный ID статуса.");
                return null;
            }

            var status = statuses.FirstOrDefault(x => x.StatusId == statusId);
            if (status == null) MenuHelper.PrintError("Статус не найден.");
            return status;
        }

        private static string Fix(string? s, int width)
        {
            s ??= "";
            if (s.Length >= width) return s.Substring(0, width);
            return s.PadRight(width, ' ');
        }
        private static string Fix(int n, int width) => Fix(n.ToString(), width);
        private static string Fix(int? n, int width) => Fix(n?.ToString() ?? "", width);

        #endregion

        #region Animal Management

        public void ListAllAnimals()
        {
            MenuHelper.PrintHeader("Список всех животных");

            var animals = _animalService.GetAllAnimals();
            if (!animals.Any())
            {
                MenuHelper.PrintInfo("Нет животных в базе данных.");
                return;
            }

            // Интерактивная настройка фильтров и сортировки
            Console.WriteLine("\n=== Фильтры (Enter - пропустить) ===");

            var filterSpecies = ReadOptional("Фильтр по виду (например: Собака, Кошка): ");
            var filterBreed = ReadOptional("Фильтр по породе: ");
            var filterGender = ReadOptional("Фильтр по полу (Male/Female/Unknown): ");
            var filterStatus = ReadOptional("Фильтр по статусу (например: Доступен, Усыновлён): ");

            Console.Write("Фильтр по возрасту от (Enter - не фильтровать): ");
            int? ageFrom = null;
            var ageFromText = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ageFromText) && int.TryParse(ageFromText, out var afVal))
                ageFrom = afVal;

            // Применение фильтров
            IEnumerable<Animal> filtered = animals;

            if (!string.IsNullOrWhiteSpace(filterSpecies))
                filtered = filtered.Where(a => a.Species != null && a.Species.Contains(filterSpecies, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filterBreed))
                filtered = filtered.Where(a => a.Breed != null && a.Breed.Contains(filterBreed, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filterGender))
                filtered = filtered.Where(a => a.Gender != null && a.Gender.Equals(filterGender, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filterStatus))
                filtered = filtered.Where(a => a.Status?.StatusName != null && a.Status.StatusName.Contains(filterStatus, StringComparison.OrdinalIgnoreCase));

            if (ageFrom.HasValue)
                filtered = filtered.Where(a => a.Age.HasValue && a.Age.Value >= ageFrom.Value);


            // Выбор сортировки
            Console.WriteLine("\n=== Сортировка ===");
            Console.WriteLine("1. ID");
            Console.WriteLine("2. Имя");
            Console.WriteLine("3. Вид");
            Console.WriteLine("4. Порода");
            Console.WriteLine("5. Возраст");
            Console.WriteLine("6. Пол");
            Console.WriteLine("7. Статус");
            Console.WriteLine("0. Без сортировки");
            Console.Write("Выберите поле для сортировки: ");
            var sortChoice = Console.ReadLine();

            Console.Write("Направление (1 - по возрастанию, 2 - по убыванию): ");
            var dirChoice = Console.ReadLine();
            bool ascending = dirChoice != "2";

            IOrderedEnumerable<Animal>? sorted = null;

            switch (sortChoice)
            {
                case "1":
                    sorted = ascending ? filtered.OrderBy(a => a.AnimalId) : filtered.OrderByDescending(a => a.AnimalId);
                    break;
                case "2":
                    sorted = ascending ? filtered.OrderBy(a => a.Name) : filtered.OrderByDescending(a => a.Name);
                    break;
                case "3":
                    sorted = ascending ? filtered.OrderBy(a => a.Species) : filtered.OrderByDescending(a => a.Species);
                    break;
                case "4":
                    sorted = ascending ? filtered.OrderBy(a => a.Breed) : filtered.OrderByDescending(a => a.Breed);
                    break;
                case "5":
                    sorted = ascending ? filtered.OrderBy(a => a.Age ?? int.MaxValue) : filtered.OrderByDescending(a => a.Age ?? int.MinValue);
                    break;
                case "6":
                    sorted = ascending ? filtered.OrderBy(a => a.Gender) : filtered.OrderByDescending(a => a.Gender);
                    break;
                case "7":
                    sorted = ascending ? filtered.OrderBy(a => a.Status?.StatusName ?? "") : filtered.OrderByDescending(a => a.Status?.StatusName ?? "");
                    break;
                default:
                    sorted = filtered.OrderBy(a => a.AnimalId); // без сортировки - по умолчанию ID
                    break;
            }

            var result = sorted.ToList();

            if (!result.Any())
            {
                MenuHelper.PrintInfo("Нет животных, соответствующих фильтрам.");
                return;
            }

            Console.WriteLine($"\nНайдено животных: {result.Count}\n");

            const int W_ID = 5, W_NAME = 20, W_SPEC = 15, W_BREED = 26, W_AGE = 7, W_GENDER = 8, W_STATUS = 20;
            var header =
                Fix("ID", W_ID) + " " +
                Fix("Имя", W_NAME) + " " +
                Fix("Вид", W_SPEC) + " " +
                Fix("Порода", W_BREED) + " " +
                Fix("Возраст", W_AGE) + " " +
                Fix("Пол", W_GENDER) + " " +
                Fix("Статус", W_STATUS);

            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));

            foreach (var a in result)
            {
                Console.WriteLine(
                    Fix(a.AnimalId, W_ID) + " " +
                    Fix(a.Name, W_NAME) + " " +
                    Fix(a.Species, W_SPEC) + " " +
                    Fix(a.Breed, W_BREED) + " " +
                    Fix(a.Age?.ToString(), W_AGE) + " " +
                    Fix(a.Gender, W_GENDER) + " " +
                    Fix(a.Status?.StatusName, W_STATUS)
                );
            }
        }

        public void AddAnimal()
        {
            MenuHelper.PrintHeader("Добавление нового животного");
            try
            {
                var animal = new Animal();
                animal.Name = ReadNonEmpty("Имя: ");
                animal.Species = ReadNonEmpty("Вид (собака, кошка и т.д.): ");
                animal.Breed = ReadOptional("Порода: ");
                animal.Age = ReadIntOptional("Возраст (лет): ");
                animal.Gender = ChooseGender("Unknown");
                animal.Description = ReadOptional("Описание: ");

                var status = ChooseAnimalStatus();
                if (status == null) return;
                animal.StatusId = status.StatusId;

                _animalService.AddAnimal(animal);
                MenuHelper.PrintSuccess($"Животное '{animal.Name}' успешно добавлено (ID={animal.AnimalId}).");
            }
            catch (DbUpdateException dbEx)
            {
                MenuHelper.PrintError($"Ошибка при добавлении животного: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Внутренняя ошибка: {dbEx.InnerException.GetType().Name}: {dbEx.InnerException.Message}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка при добавлении: {ex.Message}");
            }
        }

        public void UpdateAnimal()
        {
            MenuHelper.PrintHeader("Обновление информации о животном");
            var id = ReadIntRequired("Введите ID животного: ");
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено.");
                return;
            }

            Console.WriteLine($"\nТекущие данные: {animal.Name}, {animal.Species}, возраст {animal.Age}, пол {animal.Gender}, статус {animal.Status?.StatusName}");

            animal.Name = ReadOptional($"Имя ({animal.Name}): ", animal.Name);
            animal.Species = ReadOptional($"Вид ({animal.Species}): ", animal.Species);
            animal.Breed = ReadOptional($"Порода ({animal.Breed ?? "—"}): ", animal.Breed ?? "");
            animal.Age = ReadIntOptional($"Возраст ({animal.Age?.ToString() ?? "—"}): ", animal.Age);
            animal.Gender = ChooseGender(animal.Gender);
            animal.Description = ReadOptional($"Описание ({animal.Description ?? "—"}): ", animal.Description ?? "");

            var status = ChooseAnimalStatus(animal.StatusId);
            if (status == null) return;
            animal.StatusId = status.StatusId;

            try
            {
                _animalService.UpdateAnimal(animal);
                MenuHelper.PrintSuccess("Данные животного обновлены.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        public void DeleteAnimal()
        {
            MenuHelper.PrintHeader("Удаление животного");
            var id = ReadIntRequired("Введите ID животного: ");
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено.");
                return;
            }

            if (!Confirm($"Удалить '{animal.Name}' (ID={animal.AnimalId})?"))
            {
                MenuHelper.PrintInfo("Операция отменена.");
                return;
            }

            try
            {
                _animalService.DeleteAnimal(id);
                MenuHelper.PrintSuccess("Животное удалено.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка удаления: {ex.Message}");
            }
        }

        public void SearchAnimals()
{
    MenuHelper.PrintHeader("Поиск животных");
    Console.Write("Введите строку поиска (имя/вид/порода): ");
    var term = Console.ReadLine() ?? string.Empty;

    var result = _animalService.SearchAnimals(term);
    if (!result.Any())
    {
        MenuHelper.PrintInfo("Ничего не найдено.");
        return;
    }

    const int W_ID = 5, W_NAME = 20, W_SPEC = 15, W_BREED = 26, W_AGE = 7, W_GENDER = 8, W_STATUS = 20;
    var header =
        Fix("ID", W_ID) + " " +
        Fix("Имя", W_NAME) + " " +
        Fix("Вид", W_SPEC) + " " +
        Fix("Порода", W_BREED) + " " +
        Fix("Возраст", W_AGE) + " " +
        Fix("Пол", W_GENDER) + " " +
        Fix("Статус", W_STATUS);

    Console.WriteLine(header);
    Console.WriteLine(new string('-', header.Length));

    foreach (var a in result)
    {
        Console.WriteLine(
            Fix(a.AnimalId, W_ID) + " " +
            Fix(a.Name, W_NAME) + " " +
            Fix(a.Species, W_SPEC) + " " +
            Fix(a.Breed, W_BREED) + " " +
            Fix(a.Age?.ToString(), W_AGE) + " " +
            Fix(a.Gender, W_GENDER) + " " +
            Fix(a.Status?.StatusName, W_STATUS)
        );
    }
}

        public void ViewAnimalDetails()
        {
            MenuHelper.PrintHeader("Детальная информация о животном");
            var id = ReadIntRequired("Введите ID животного: ");
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено.");
                return;
            }

            Console.WriteLine($"ID: {animal.AnimalId}");
            Console.WriteLine($"Имя: {animal.Name}");
            Console.WriteLine($"Вид: {animal.Species}");
            Console.WriteLine($"Порода: {animal.Breed}");
            Console.WriteLine($"Возраст: {animal.Age}");
            Console.WriteLine($"Пол: {animal.Gender}");
            Console.WriteLine($"Статус: {animal.Status?.StatusName}");
            Console.WriteLine($"Описание: {animal.Description}");
            Console.WriteLine($"Поступил: {animal.DateAdmitted:dd.MM.yyyy}");
            Console.WriteLine();

            var med = _medicalService.GetMedicalRecordByAnimalId(animal.AnimalId);
            if (med == null)
            {
                MenuHelper.PrintInfo("Медицинская карта отсутствует.");
                return;
            }

            Console.WriteLine($"Мед. карта #{med.RecordNumber}, создана: {med.CreatedDate:dd.MM.yyyy}");
            if (!string.IsNullOrWhiteSpace(med.BloodType)) Console.WriteLine($"Группа крови: {med.BloodType}");
            if (!string.IsNullOrWhiteSpace(med.Allergies)) Console.WriteLine($"Аллергии: {med.Allergies}");
            if (!string.IsNullOrWhiteSpace(med.ChronicConditions)) Console.WriteLine($"Хронические: {med.ChronicConditions}");
            if (!string.IsNullOrWhiteSpace(med.SpecialNeeds)) Console.WriteLine($"Особые нужды: {med.SpecialNeeds}");
            if (!string.IsNullOrWhiteSpace(med.VeterinarianNotes)) Console.WriteLine($"Заметки ветеринара: {med.VeterinarianNotes}");
            Console.WriteLine();

            var vacs = _medicalService.GetVaccinationsByAnimal(animal.AnimalId);
            if (!vacs.Any())
            {
                MenuHelper.PrintInfo("Прививки не зарегистрированы.");
                return;
            }

            Console.WriteLine($"{"Дата",-12} {"Вакцина",-25} {"Партия",-15} {"Ветврач",-20} {"След. дата",-12}");
            Console.WriteLine(new string('-', 90));
            foreach (var v in vacs)
            {
                Console.WriteLine($"{v.VaccinationDate:dd.MM.yyyy,-12} {v.Vaccination?.VaccineName,-25} {v.BatchNumber,-15} {v.VeterinarianName,-20} {v.NextDueDate:dd.MM.yyyy,-12}");
            }
        }

        #endregion

        #region Adopter Management

        public void ListAllAdopters()
        {
            MenuHelper.PrintHeader("Список усыновителей");
            var list = _adopterService.GetAllAdopters();
            if (!list.Any())
            {
                MenuHelper.PrintInfo("Усыновителей нет.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Имя",-15} {"Фамилия",-18} {"E-mail",-28} {"Телефон",-16} {"Статус",-10}");
            Console.WriteLine(new string('-', 100));
            foreach (var a in list)
            {
                Console.WriteLine($"{a.AdopterId,-5} {a.FirstName,-15} {a.LastName,-18} {a.Email,-28} {a.Phone,-16} {a.ApprovalStatus,-10}");
            }
        }

        public void AddAdopter()
        {
            MenuHelper.PrintHeader("Регистрация нового усыновителя");

            while (true)
            {
                try
                {
                    var adopter = new Adopter
                    {
                        FirstName = ReadNonEmpty("Имя: "),
                        LastName = ReadNonEmpty("Фамилия: "),
                        Email = ReadValidatedEmailRequired("E-mail: "),
                        Phone = ReadValidatedPhoneRequired("Телефон (пример: +79161234567): "),
                        Address = ReadNonEmpty("Адрес: ")
                    };

                    // Сводка и подтверждение
                    if (!ConfirmAdopterSummary(adopter))
                    {
                        MenuHelper.PrintInfo("Повторный ввод данных.");
                        continue;
                    }

                    _adopterService.AddAdopter(adopter);
                    MenuHelper.PrintSuccess($"Усыновитель зарегистрирован (ID={adopter.AdopterId}).");
                    break;
                }
                catch (ArgumentException aex)
                {
                    // Серверная валидация на всякий случай (дублирует клиентскую)
                    MenuHelper.PrintError(aex.Message);
                    MenuHelper.PrintInfo("Попробуйте ввести данные заново.");
                }
                catch (Exception ex)
                {
                    MenuHelper.PrintError($"Ошибка: {ex.Message}");
                    break;
                }
            }
        }

        public void UpdateAdopter()
        {
            MenuHelper.PrintHeader("Обновить данные усыновителя");
            var id = ReadIntRequired("Введите ID усыновителя: ");
            var adopter = _adopterService.GetAdopterById(id);
            if (adopter == null)
            {
                MenuHelper.PrintError("Усыновитель не найден.");
                return;
            }

            while (true)
            {
                // Показ текущих значений в подсказках
                adopter.FirstName = ReadOptional($"Имя ({adopter.FirstName}): ", adopter.FirstName);
                adopter.LastName = ReadOptional($"Фамилия ({adopter.LastName}): ", adopter.LastName);
                adopter.Email = ReadValidatedEmailOptional($"E-mail ({adopter.Email}): ", adopter.Email);
                adopter.Phone = ReadValidatedPhoneOptional($"Телефон ({adopter.Phone}): ", adopter.Phone);
                adopter.Address = ReadOptional($"Адрес ({adopter.Address}): ", adopter.Address);

                // Сводка и подтверждение
                if (!ConfirmAdopterSummary(adopter))
                {
                    MenuHelper.PrintInfo("Повторный ввод данных.");
                    continue;
                }

                try
                {
                    _adopterService.UpdateAdopter(adopter);
                    MenuHelper.PrintSuccess("Данные усыновителя обновлены.");
                    break;
                }
                catch (ArgumentException aex)
                {
                    MenuHelper.PrintError(aex.Message);
                    MenuHelper.PrintInfo("Попробуйте ввести данные заново.");
                }
                catch (Exception ex)
                {
                    MenuHelper.PrintError($"Ошибка: {ex.Message}");
                    break;
                }
            }
        }

        private static string ReadValidatedEmailRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var email = (Console.ReadLine() ?? string.Empty).Trim();
                if (AdopterService.ValidateEmail(email))
                    return email;

                MenuHelper.PrintError("Некорректный e-mail. Пример: example@gmail.com");
            }
        }

        private static string ReadValidatedPhoneRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var phone = (Console.ReadLine() ?? string.Empty).Trim();
                if (AdopterService.ValidatePhone(phone))
                    return phone;

                MenuHelper.PrintError("Некорректный телефон. Примеры: +79161234567, 84951234567, +44...");
            }
        }

        private static string ReadValidatedEmailOptional(string prompt, string current)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    return current; // оставить без изменений

                var email = input.Trim();
                if (AdopterService.ValidateEmail(email))
                    return email;

                MenuHelper.PrintError("Некорректный e-mail. Пример: example@gmail.com");
            }
        }

        private static string ReadValidatedPhoneOptional(string prompt, string current)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    return current; // оставить без изменений

                var phone = input.Trim();
                if (AdopterService.ValidatePhone(phone))
                    return phone;

                MenuHelper.PrintError("Некорректный телефон. Примеры: +79161234567, 84951234567, +44...");
            }
        }

        private static bool ConfirmAdopterSummary(Adopter a)
        {
            Console.WriteLine("\nПроверьте введённые данные:");
            Console.WriteLine($"Имя: {a.FirstName}");
            Console.WriteLine($"Фамилия: {a.LastName}");
            Console.WriteLine($"E-mail: {a.Email}");
            Console.WriteLine($"Телефон: {a.Phone}");
            Console.WriteLine($"Адрес: {a.Address}");
            return Confirm("Всё правильно");
        }


        public void ViewAdopterDetails()
        {
            MenuHelper.PrintHeader("Детальная информация об усыновителе");
            var id = ReadIntRequired("Введите ID усыновителя: ");
            var adopter = _adopterService.GetAdopterById(id);
            if (adopter == null)
            {
                MenuHelper.PrintError("Усыновитель не найден.");
                return;
            }

            Console.WriteLine($"ID: {adopter.AdopterId}");
            Console.WriteLine($"Имя: {adopter.FirstName}");
            Console.WriteLine($"Фамилия: {adopter.LastName}");
            Console.WriteLine($"E-mail: {adopter.Email}");
            Console.WriteLine($"Телефон: {adopter.Phone}");
            Console.WriteLine($"Адрес: {adopter.Address}");
            Console.WriteLine($"Статус: {adopter.ApprovalStatus}");
            Console.WriteLine();

            var adoptions = adopter.Adoptions?.ToList() ?? new List<Adoption>();
            if (!adoptions.Any())
            {
                MenuHelper.PrintInfo("История усыновлений отсутствует.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Животное",-20} {"Дата",-12} {"Статус",-18} {"Возврат",-12} {"Заметки",-25}");
            Console.WriteLine(new string('-', 100));
            foreach (var ad in adoptions)
            {
                var returnDate = ad.ReturnDate?.ToString("dd.MM.yyyy") ?? "—";
                Console.WriteLine($"{ad.AdoptionId,-5} {ad.Animal?.Name,-20} {ad.AdoptionDate:dd.MM.yyyy,-12} {ad.Status?.StatusName,-18} {returnDate,-12} {ad.Notes,-25}");
            }
        }

        #endregion

        #region Adoption Management

        public void ListAllAdoptions()
        {
            MenuHelper.PrintHeader("Список усыновлений");
            var list = _adoptionService.GetAllAdoptions();
            if (!list.Any())
            {
                MenuHelper.PrintInfo("Записей об усыновлениях нет.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Усыновитель",-25} {"Животное",-20} {"Дата",-12} {"Статус",-18} {"Возврат",-12}");
            Console.WriteLine(new string('-', 100));
            foreach (var ad in list)
            {
                var adopterName = ad.Adopter != null ? $"{ad.Adopter.FirstName} {ad.Adopter.LastName}" : "-";
                var returnDate = ad.ReturnDate?.ToString("dd.MM.yyyy") ?? "—";
                Console.WriteLine($"{ad.AdoptionId,-5} {adopterName,-25} {ad.Animal?.Name,-20} {ad.AdoptionDate:dd.MM.yyyy,-12} {ad.Status?.StatusName,-18} {returnDate,-12}");
            }
        }

       public void CreateAdoption()
        {
            MenuHelper.PrintHeader("Создание записи об усыновлении");

            var adopterId = ReadIntRequired("ID усыновителя: ");
            var adopter = _adopterService.GetAdopterById(adopterId);
            if (adopter == null)
            {
                MenuHelper.PrintError("Усыновитель не найден.");
                return;
            }

            var animalId = ReadIntRequired("ID животного: ");
            var animal = _animalService.GetAnimalById(animalId);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено.");
                return;
            }

            var status = ChooseAdoptionStatus();
            if (status == null) return;

            var adoption = new Adoption
            {
                AdopterId = adopterId,
                AnimalId = animalId,
                StatusId = status.StatusId,
                Notes = ReadOptional("Заметки (необязательно): ")
            };

            Console.WriteLine("\nПроверьте данные:");
            Console.WriteLine($"Усыновитель: {adopter.FirstName} {adopter.LastName} (ID={adopter.AdopterId})");
            Console.WriteLine($"Животное: {animal.Name} (ID={animal.AnimalId})");
            Console.WriteLine($"Статус: {status.StatusName} (ID={status.StatusId})");
            Console.WriteLine($"Заметки: {adoption.Notes}");
            if (!Confirm("Всё правильно")) { MenuHelper.PrintInfo("Операция отменена."); return; }

            try
            {
                _adoptionService.CreateAdoption(adoption);
                MenuHelper.PrintSuccess($"Запись создана (ID={adoption.AdoptionId}).");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                MenuHelper.PrintError($"Ошибка сохранения: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Внутренняя ошибка: {dbEx.InnerException.GetType().Name}: {dbEx.InnerException.Message}");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка создания: {ex.Message}");
            }
        }
        public void UpdateAdoption()
        {
            MenuHelper.PrintHeader("Обновление статуса усыновления");
            var adoptionId = ReadIntRequired("Введите ID записи: ");
            var adoption = _adoptionService.GetAdoptionById(adoptionId);
            if (adoption == null)
            {
                MenuHelper.PrintError("Запись об усыновлении не найдена.");
                return;
            }

            Console.WriteLine($"Текущий статус: {adoption.Status?.StatusName}");
            var status = ChooseAdoptionStatus(adoption.StatusId);
            if (status == null) return;

            adoption.StatusId = status.StatusId;
            adoption.Notes = ReadOptional($"Заметки ({adoption.Notes ?? "—"}): ", adoption.Notes ?? "");

            Console.Write("Дата возврата (дд.мм.гггг, Enter — без изменений/очистить): ");
            var rdText = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(rdText))
            {
                // без изменений
            }
            else if (rdText.Trim().Equals("-", StringComparison.OrdinalIgnoreCase))
            {
                adoption.ReturnDate = null;
            }
            else if (DateTime.TryParse(rdText, out var rd))
            {
                adoption.ReturnDate = rd;
            }

            try
            {
                _adoptionService.UpdateAdoption(adoption);
                MenuHelper.PrintSuccess("Статус усыновления обновлён.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        #endregion

        #region Medical

        public void ViewMedicalRecord()
            {
                MenuHelper.PrintHeader("Просмотр медицинской карты");

                // Локальные переменные, видимые во всём методе
                var animals = _animalService.GetAllAnimals();
                var idsWith = _medicalService.GetAnimalIdsWithMedicalRecords();

                if (!animals.Any())
                {
                    MenuHelper.PrintInfo("Животных нет в базе.");
                    return;
                }

                const int W_ID = 5, W_NAME = 20, W_BREED = 20, W_HAS = 8;
                var header = $"{Fix("ID", W_ID)} {Fix("Имя", W_NAME)} {Fix("Порода", W_BREED)} {Fix("Карта", W_HAS)}";
                Console.WriteLine(header);
                Console.WriteLine(new string('-', header.Length));

                foreach (var a in animals)
                {
                    var has = idsWith.Contains(a.AnimalId) ? "Есть" : "Нет";
                    Console.WriteLine($"{Fix(a.AnimalId, W_ID)} {Fix(a.Name, W_NAME)} {Fix(a.Breed, W_BREED)} {Fix(has, W_HAS)}");
                }

                var animalId = ReadIntRequired("ID животного: ");
                var record = _medicalService.GetMedicalRecordByAnimalId(animalId);
                if (record == null)
                {
                    MenuHelper.PrintInfo("Медицинская карта не найдена.");
                    return;
                }

                Console.WriteLine($"Животное: {record.Animal?.Name} (ID={record.AnimalId})");
                Console.WriteLine($"№ карты: {record.RecordNumber}");
                Console.WriteLine($"Создана: {record.CreatedDate:dd.MM.yyyy}");
                if (!string.IsNullOrWhiteSpace(record.BloodType)) Console.WriteLine($"Группа крови: {record.BloodType}");
                if (!string.IsNullOrWhiteSpace(record.Allergies)) Console.WriteLine($"Аллергии: {record.Allergies}");
                if (!string.IsNullOrWhiteSpace(record.ChronicConditions)) Console.WriteLine($"Хронические заболевания: {record.ChronicConditions}");
                if (!string.IsNullOrWhiteSpace(record.SpecialNeeds)) Console.WriteLine($"Особые нужды: {record.SpecialNeeds}");
                if (!string.IsNullOrWhiteSpace(record.VeterinarianNotes)) Console.WriteLine($"Заметки врача: {record.VeterinarianNotes}");
                if (record.LastCheckupDate.HasValue) Console.WriteLine($"Последний осмотр: {record.LastCheckupDate:dd.MM.yyyy}");

                Console.WriteLine();
                var vacs = _medicalService.GetVaccinationsByAnimal(animalId);
                if (!vacs.Any())
                {
                    MenuHelper.PrintInfo("Прививки отсутствуют.");
                    return;
                }

                const int W_DATE = 12, W_VAC = 25, W_BATCH = 12, W_VET = 20, W_NEXT = 12, W_NOTES = 25;
                var vHeader = $"{Fix("Дата", W_DATE)} {Fix("Вакцина", W_VAC)} {Fix("Партия", W_BATCH)} {Fix("Ветврач", W_VET)} {Fix("След. дата", W_NEXT)} {Fix("Примечания", W_NOTES)}";
                Console.WriteLine(vHeader);
                Console.WriteLine(new string('-', vHeader.Length));

                foreach (var v in vacs)
                {
                    var next = v.NextDueDate?.ToString("dd.MM.yyyy") ?? "—";
                    Console.WriteLine($"{Fix(v.VaccinationDate.ToString("dd.MM.yyyy"), W_DATE)} {Fix(v.Vaccination?.VaccineName ?? "", W_VAC)} {Fix(v.BatchNumber ?? "", W_BATCH)} {Fix(v.VeterinarianName ?? "", W_VET)} {Fix(next, W_NEXT)} {Fix(v.Notes ?? "", W_NOTES)}");
                }
            }

            public void CreateMedicalRecord()
            {
                MenuHelper.PrintHeader("Создание медицинской карты");

                var candidates = _medicalService.GetAnimalsWithoutMedicalRecord();
                if (!candidates.Any())
                {
                    MenuHelper.PrintInfo("Все животные уже имеют медицинские карты.");
                    return;
                }

                const int W_ID = 5, W_NAME = 20, W_BREED = 20;
                var header = $"{Fix("ID", W_ID)} {Fix("Имя", W_NAME)} {Fix("Порода", W_BREED)}";
                Console.WriteLine(header);
                Console.WriteLine(new string('-', header.Length));
                foreach (var a in candidates)
                    Console.WriteLine($"{Fix(a.AnimalId, W_ID)} {Fix(a.Name, W_NAME)} {Fix(a.Breed, W_BREED)}");

                var animalId = ReadIntRequired("Выберите ID животного без карты: ");
                if (_medicalService.GetMedicalRecordByAnimalId(animalId) != null)
                {
                    MenuHelper.PrintError("Для этого животного карта уже существует.");
                    return;
                }

                while (true)
                {
                    var recNumInput = ReadNonEmpty("Номер карты (например, MR-0001 или 0001): ");
                    var recNum = NormalizeMedicalRecordNumber(recNumInput);
                    Console.WriteLine($"Нормализованный номер: {recNum}");
                    if (_medicalService.RecordNumberExists(recNum))
                    {
                        MenuHelper.PrintError("Карта с таким номером уже существует. Введите другой номер.");
                        continue;
                    }

                    var record = new MedicalRecord
                    {
                        AnimalId = animalId,
                        RecordNumber = recNum,
                        BloodType = ReadOptional("Группа крови (необязательно): "),
                        Allergies = ReadOptional("Аллергии (необязательно): "),
                        ChronicConditions = ReadOptional("Хронические заболевания (необязательно): "),
                        SpecialNeeds = ReadOptional("Особые нужды (необязательно): "),
                        VeterinarianNotes = ReadOptional("Заметки ветеринара (необязательно): "),
                        LastCheckupDate = ReadDateOptional("Последний осмотр (дд.мм.гггг, необязательно): ", null)
                    };

                    Console.WriteLine("\nПроверьте данные:");
                    Console.WriteLine($"№ карты: {record.RecordNumber}");
                    Console.WriteLine($"Группа крови: {record.BloodType}");
                    Console.WriteLine($"Аллергии: {record.Allergies}");
                    Console.WriteLine($"Хронические: {record.ChronicConditions}");
                    Console.WriteLine($"Особые нужды: {record.SpecialNeeds}");
                    Console.WriteLine($"Заметки: {record.VeterinarianNotes}");
                    Console.WriteLine($"Последний осмотр: {(record.LastCheckupDate.HasValue ? record.LastCheckupDate.Value.ToString("dd.MM.yyyy") : "—")}");

                    if (!Confirm("Всё правильно"))
                    {
                        if (!Confirm("Повторить ввод?"))
                        {
                            MenuHelper.PrintInfo("Операция отменена.");
                            return;
                        }
                        continue;
                    }

                    try
                    {
                        _medicalService.CreateMedicalRecord(record);
                        MenuHelper.PrintSuccess("Медицинская карта создана.");
                        return;
                    }
                    catch (ArgumentException aex)
                    {
                        MenuHelper.PrintError(aex.Message);
                        if (!Confirm("Повторить ввод?")) return;
                    }
                    catch (Exception ex)
                    {
                        MenuHelper.PrintError($"Ошибка создания карты: {ex.Message}");
                        return;
                    }
                }
            }

        public void EditMedicalRecord()
        {
            MenuHelper.PrintHeader("Редактирование медицинской карты");

            var animals = _animalService.GetAllAnimals();
            if (!animals.Any())
            {
                MenuHelper.PrintInfo("Животных нет в базе.");
                return;
            }

            const int W_ID = 5, W_NAME = 20, W_BREED = 20;
            var header = $"{Fix("ID", W_ID)} {Fix("Имя", W_NAME)} {Fix("Порода", W_BREED)}";
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
            foreach (var a in animals)
                Console.WriteLine($"{Fix(a.AnimalId, W_ID)} {Fix(a.Name, W_NAME)} {Fix(a.Breed, W_BREED)}");

            var animalId = ReadIntRequired("Введите ID животного: ");
            var record = _medicalService.GetMedicalRecordByAnimalId(animalId);
            if (record == null)
            {
                MenuHelper.PrintError("Медицинская карта не найдена.");
                return;
            }

            while (true)
            {
                var newNumberInput = ReadOptional($"Номер карты ({record.RecordNumber}): ", record.RecordNumber);
                var newNumber = NormalizeMedicalRecordNumber(newNumberInput);
                if (newNumber != record.RecordNumber)
                    Console.WriteLine($"Нормализованный номер: {newNumber}");
                if (!string.Equals(newNumber, record.RecordNumber, StringComparison.OrdinalIgnoreCase)
                    && _medicalService.RecordNumberExistsForOther(newNumber, record.AnimalId))
                {
                    MenuHelper.PrintError("Карта с таким номером уже существует. Введите другой номер.");
                    continue;
                }

                var newBlood = ReadOptional($"Группа крови ({record.BloodType ?? "—"}): ", record.BloodType ?? "");
                var newAll = ReadOptional($"Аллергии ({record.Allergies ?? "—"}): ", record.Allergies ?? "");
                var newChronic = ReadOptional($"Хронические заболевания ({record.ChronicConditions ?? "—"}): ", record.ChronicConditions ?? "");
                var newNeeds = ReadOptional($"Особые нужды ({record.SpecialNeeds ?? "—"}): ", record.SpecialNeeds ?? "");
                var newNotes = ReadOptional($"Заметки ветеринара ({record.VeterinarianNotes ?? "—"}): ", record.VeterinarianNotes ?? "");
                var newCheck = ReadDateOptional($"Последний осмотр ({(record.LastCheckupDate.HasValue ? record.LastCheckupDate.Value.ToString("dd.MM.yyyy") : "—")}): ", record.LastCheckupDate);

                Console.WriteLine("\nПроверьте данные:");
                Console.WriteLine($"№ карты: {newNumber}");
                Console.WriteLine($"Группа крови: {newBlood}");
                Console.WriteLine($"Аллергии: {newAll}");
                Console.WriteLine($"Хронические: {newChronic}");
                Console.WriteLine($"Особые нужды: {newNeeds}");
                Console.WriteLine($"Заметки: {newNotes}");
                Console.WriteLine($"Последний осмотр: {(newCheck.HasValue ? newCheck.Value.ToString("dd.MM.yyyy") : "—")}");

                if (!Confirm("Сохранить изменения"))
                {
                    if (!Confirm("Повторить ввод?"))
                    {
                        MenuHelper.PrintInfo("Изменения отменены.");
                        return;
                    }
                    continue;
                }

                record.RecordNumber = newNumber;
                record.BloodType = newBlood;
                record.Allergies = newAll;
                record.ChronicConditions = newChronic;
                record.SpecialNeeds = newNeeds;
                record.VeterinarianNotes = newNotes;
                record.LastCheckupDate = newCheck;

                try
                {
                    _medicalService.UpdateMedicalRecord(record);
                    MenuHelper.PrintSuccess("Медицинская карта обновлена.");
                    return;
                }
                catch (ArgumentException aex)
                {
                    MenuHelper.PrintError(aex.Message);
                    if (!Confirm("Повторить ввод?")) return;
                }
                catch (Exception ex)
                {
                    MenuHelper.PrintError($"Ошибка сохранения: {ex.Message}");
                    return;
                }
            }
        }

        public void AddVaccinationRecord()
        {
            MenuHelper.PrintHeader("Добавление записи о вакцинации");
            try
            {
                var animalId = ReadIntRequired("ID животного: ");

                var med = _medicalService.GetMedicalRecordByAnimalId(animalId);
                if (med == null)
                {
                    MenuHelper.PrintError("Для животного нет медицинской карты. Сначала создайте её.");
                    return;
                }

                var vaccinations = _medicalService.GetAllVaccinations();
                if (!vaccinations.Any())
                {
                    MenuHelper.PrintError("Справочник вакцин пуст. Добавьте вакцину.");
                    return;
                }

                Console.WriteLine("\nДоступные вакцины:");
                foreach (var v in vaccinations)
                    Console.WriteLine($"{v.VaccinationId}. {v.VaccineName}");

                var vacId = ReadIntRequired("Выберите ID вакцины: ");
                var batch = ReadOptional("Номер партии (необязательно): ");
                var vet = ReadOptional("Имя ветеринара (необязательно): ");
                var nextDue = ReadDateOptional("Дата следующей вакцинации (дд.мм.гггг, необязательно): ", null);
                var notes = ReadOptional("Примечания (необязательно): ");

                var rec = new AnimalVaccination
                {
                    AnimalId = animalId,
                    VaccinationId = vacId,
                    BatchNumber = batch,
                    VeterinarianName = vet,
                    NextDueDate = nextDue,
                    Notes = notes
                };

                _medicalService.AddAnimalVaccination(rec);
                MenuHelper.PrintSuccess("Запись о вакцинации добавлена.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        public void ViewOverdueVaccinations()
        {
            MenuHelper.PrintHeader("Просроченные вакцинации");

            var overdue = _medicalService.GetOverdueVaccinations();
            if (!overdue.Any())
            {
                MenuHelper.PrintSuccess("Нет просроченных вакцинаций.");
                return;
            }

            Console.WriteLine($"{"ID зап.",-8} {"Животное",-20} {"Вакцина",-25} {"Срок истёк",-12}");
            Console.WriteLine(new string('-', 70));
            foreach (var v in overdue)
            {
                var due = v.NextDueDate?.ToString("dd.MM.yyyy") ?? "—";
                Console.WriteLine($"{v.VaccinationRecordId,-8} {v.MedicalRecord?.Animal?.Name,-20} {v.Vaccination?.VaccineName,-25} {due,-12}");
            }

            Console.WriteLine();
            var vrIdText = ReadOptional("Введите ID записи для редактирования/удаления (Enter/0 — выход): ", "");
            if (string.IsNullOrWhiteSpace(vrIdText) || vrIdText.Trim() == "0")
                return;

            if (!int.TryParse(vrIdText, out var vrId))
            {
                MenuHelper.PrintError("Некорректный ID.");
                return;
            }

            EditVaccinationRecord(vrId);
        }

        public void EditVaccinationRecord(int vaccinationRecordId)
        {
            var rec = _medicalService.GetVaccinationRecordById(vaccinationRecordId);
            if (rec == null)
            {
                MenuHelper.PrintError("Запись вакцинации не найдена.");
                return;
            }

            Console.WriteLine($"\nЖивотное: {rec.MedicalRecord?.Animal?.Name} (ID={rec.AnimalId})");
            Console.WriteLine($"Текущая вакцина: {rec.Vaccination?.VaccineName} (ID={rec.VaccinationId})");
            Console.WriteLine($"Партия: {rec.BatchNumber ?? "—"}");
            Console.WriteLine($"Ветврач: {rec.VeterinarianName ?? "—"}");
            Console.WriteLine($"След. дата: {(rec.NextDueDate.HasValue ? rec.NextDueDate.Value.ToString("dd.MM.yyyy") : "—")}");
            Console.WriteLine($"Примечания: {rec.Notes ?? "—"}");

            Console.WriteLine();
            if (Confirm("Удалить эту запись?"))
            {
                if (Confirm("Подтвердите удаление"))
                {
                    _medicalService.DeleteAnimalVaccination(rec.VaccinationRecordId);
                    MenuHelper.PrintSuccess("Запись вакцинации удалена.");
                }
                else
                {
                    MenuHelper.PrintInfo("Удаление отменено.");
                }
                return;
            }

            // Выбор вакцины (опционально)
            var vaccinations = _medicalService.GetAllVaccinations();
            Console.WriteLine("\nДоступные вакцины:");
            foreach (var v in vaccinations)
                Console.WriteLine($"{v.VaccinationId}. {v.VaccineName}");

            var newVacIdText = ReadOptional($"ID вакцины ({rec.VaccinationId}): ", rec.VaccinationId.ToString());
            if (int.TryParse(newVacIdText, out var newVacId))
                rec.VaccinationId = newVacId;

            rec.BatchNumber = ReadOptional($"Партия ({rec.BatchNumber ?? "—"}): ", rec.BatchNumber ?? "");
            rec.VeterinarianName = ReadOptional($"Ветврач ({rec.VeterinarianName ?? "—"}): ", rec.VeterinarianName ?? "");
            rec.NextDueDate = ReadDateOptional($"Следующая дата ({(rec.NextDueDate.HasValue ? rec.NextDueDate.Value.ToString("dd.MM.yyyy") : "—")}): ", rec.NextDueDate);
            rec.Notes = ReadOptional($"Примечания ({rec.Notes ?? "—"}): ", rec.Notes ?? "");

            Console.WriteLine("\nПроверьте изменения:");
            Console.WriteLine($"Вакцина ID: {rec.VaccinationId}");
            Console.WriteLine($"Партия: {rec.BatchNumber}");
            Console.WriteLine($"Ветврач: {rec.VeterinarianName}");
            Console.WriteLine($"След. дата: {(rec.NextDueDate.HasValue ? rec.NextDueDate.Value.ToString("dd.MM.yyyy") : "—")}");
            Console.WriteLine($"Примечания: {rec.Notes}");

            if (!Confirm("Сохранить изменения"))
            {
                MenuHelper.PrintInfo("Изменения отменены.");
                return;
            }

            try
            {
                _medicalService.UpdateAnimalVaccination(rec);
                MenuHelper.PrintSuccess("Запись вакцинации обновлена.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка сохранения: {ex.Message}");
            }
        }

        public void AddVaccination()
        {
            MenuHelper.PrintHeader("Добавление вакцины");
            try
            {
                var v = new Vaccination
                {
                    VaccineName = ReadNonEmpty("Название вакцины: "),
                    Description = ReadOptional("Описание (необязательно): "),
                    Manufacturer = ReadOptional("Производитель (необязательно): "),
                    ValidityPeriodMonths = ReadIntOptional("Срок действия (мес., необязательно): ", null)
                };

                _medicalService.AddVaccination(v);
                MenuHelper.PrintSuccess("Вакцина добавлена.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        #endregion

        #region Reports

        public void GenerateAnimalReport()
        {
            MenuHelper.PrintHeader("Отчет по животным");
            var animals = _animalService.GetAllAnimals();
            var statuses = _animalService.GetAllStatuses();

            Console.WriteLine($"Всего животных в приюте: {animals.Count}\n");

            Console.WriteLine("Распределение по статусам:");
            foreach (var s in statuses)
            {
                var count = animals.Count(a => a.StatusId == s.StatusId);
                Console.WriteLine($" {s.StatusName,-20}: {count}");
            }

            Console.WriteLine("\nРаспределение по видам:");
            foreach (var g in animals.GroupBy(a => a.Species))
            {
                Console.WriteLine($" {g.Key,-20}: {g.Count()}");
            }

            var withAge = animals.Where(a => a.Age.HasValue).ToList();
            if (withAge.Any())
            {
                var avgAge = withAge.Average(a => a.Age!.Value);
                Console.WriteLine($"\nСредний возраст: {avgAge:F1} лет");
            }
            else
            {
                Console.WriteLine("\nСредний возраст: —");
            }
        }

        public void GenerateAdoptionReport()
        {
            MenuHelper.PrintHeader("Отчет по усыновлениям");
            var adoptions = _adoptionService.GetAllAdoptions();
            var statuses = _adoptionService.GetAllStatuses();

            Console.WriteLine($"Всего записей об усыновлениях: {adoptions.Count}\n");

            Console.WriteLine("Распределение по статусам:");
            foreach (var s in statuses)
            {
                var count = adoptions.Count(a => a.StatusId == s.StatusId);
                Console.WriteLine($" {s.StatusName,-20}: {count}");
            }

            var now = DateTime.Now;
            var thisMonth = adoptions.Count(a => a.AdoptionDate.Month == now.Month && a.AdoptionDate.Year == now.Year);
            var thisYear = adoptions.Count(a => a.AdoptionDate.Year == now.Year);

            Console.WriteLine($"\nУсыновлений в текущем месяце: {thisMonth}");
            Console.WriteLine($"Усыновлений в текущем году: {thisYear}");
        }

        public void ListAllVaccinations()
        {
            MenuHelper.PrintHeader("Список вакцин");

            var list = _medicalService.GetAllVaccinations();
            if (list == null || !list.Any())
            {
                MenuHelper.PrintInfo("Вакцины отсутствуют.");
                return;
            }

            Console.WriteLine($"{ "ID",-6} {"Название",-40} {"Описание",-40}");
            Console.WriteLine(new string('-', 90));
            foreach (var v in list)
            {
                var desc = string.IsNullOrWhiteSpace(v.Description) ? "—" : v.Description;
                Console.WriteLine($"{v.VaccinationId,-6} {v.VaccineName,-40} {desc,-40}");
            }
        }
    

        #endregion
    }
}
