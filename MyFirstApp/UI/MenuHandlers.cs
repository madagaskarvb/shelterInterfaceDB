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

            Console.WriteLine($"{ "ID",-5} {"Имя",-20} {"Вид",-15} {"Порода",-15} {"Возраст",-8} {"Пол",-8} {"Статус",-20}");
            Console.WriteLine(new string('-', 100));
            foreach (var a in animals)
            {
                Console.WriteLine($"{a.AnimalId,-5} {a.Name,-20} {a.Species,-15} {a.Breed,-15} {a.Age,-8} {a.Gender,-8} {a.Status?.StatusName,-20}");
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

            Console.WriteLine($"{ "ID",-5} {"Имя",-20} {"Вид",-15} {"Порода",-15} {"Возраст",-8} {"Пол",-8} {"Статус",-20}");
            Console.WriteLine(new string('-', 100));
            foreach (var a in result)
            {
                Console.WriteLine($"{a.AnimalId,-5} {a.Name,-20} {a.Species,-15} {a.Breed,-15} {a.Age,-8} {a.Gender,-8} {a.Status?.StatusName,-20}");
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

            Console.WriteLine($"{ "Дата",-12} {"Вакцина",-25} {"Партия",-15} {"Ветврач",-20} {"След. дата",-12}");
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

            Console.WriteLine($"{ "ID",-5} {"Имя",-15} {"Фамилия",-18} {"E-mail",-28} {"Телефон",-16} {"Статус",-10}");
            Console.WriteLine(new string('-', 100));
            foreach (var a in list)
            {
                Console.WriteLine($"{a.AdopterId,-5} {a.FirstName,-15} {a.LastName,-18} {a.Email,-28} {a.Phone,-16} {a.ApprovalStatus,-10}");
            }
        }

        public void AddAdopter()
        {
            MenuHelper.PrintHeader("Регистрация нового усыновителя");
            try
            {
                var adopter = new Adopter
                {
                    FirstName = ReadNonEmpty("Имя: "),
                    LastName = ReadNonEmpty("Фамилия: "),
                    Email = ReadNonEmpty("E-mail: "),
                    Phone = ReadNonEmpty("Телефон (+7..., 8...): "),
                    Address = ReadNonEmpty("Адрес: ")
                };

                _adopterService.AddAdopter(adopter);
                MenuHelper.PrintSuccess($"Усыновитель добавлен (ID={adopter.AdopterId}).");
            }
            catch (ArgumentException aex)
            {
                MenuHelper.PrintError(aex.Message);
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка добавления: {ex.Message}");
            }
        }

        public void UpdateAdopter()
        {
            MenuHelper.PrintHeader("Обновление данных усыновителя");
            var id = ReadIntRequired("Введите ID усыновителя: ");
            var adopter = _adopterService.GetAdopterById(id);
            if (adopter == null)
            {
                MenuHelper.PrintError("Усыновитель не найден.");
                return;
            }

            adopter.FirstName = ReadOptional($"Имя ({adopter.FirstName}): ", adopter.FirstName);
            adopter.LastName = ReadOptional($"Фамилия ({adopter.LastName}): ", adopter.LastName);
            adopter.Email = ReadOptional($"E-mail ({adopter.Email}): ", adopter.Email);
            adopter.Phone = ReadOptional($"Телефон ({adopter.Phone}): ", adopter.Phone);
            adopter.Address = ReadOptional($"Адрес ({adopter.Address}): ", adopter.Address);

            try
            {
                _adopterService.UpdateAdopter(adopter);
                MenuHelper.PrintSuccess("Данные усыновителя обновлены.");
            }
            catch (ArgumentException aex)
            {
                MenuHelper.PrintError(aex.Message);
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
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

            Console.WriteLine($"{ "ID",-5} {"Животное",-20} {"Дата",-12} {"Статус",-18} {"Возврат",-12} {"Заметки",-25}");
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

            Console.WriteLine($"{ "ID",-5} {"Усыновитель",-25} {"Животное",-20} {"Дата",-12} {"Статус",-18} {"Возврат",-12}");
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
            var animalId = ReadIntRequired("ID животного: ");

            var status = ChooseAdoptionStatus();
            if (status == null) return;

            var adoption = new Adoption
            {
                AdopterId = adopterId,
                AnimalId = animalId,
                StatusId = status.StatusId,
                Notes = ReadOptional("Заметки (необязательно): ")
            };

            try
            {
                _adoptionService.CreateAdoption(adoption);
                MenuHelper.PrintSuccess($"Запись создана (ID={adoption.AdoptionId}).");
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

            Console.WriteLine($"{ "Дата",-12} {"Вакцина",-25} {"Партия",-12} {"Ветврач",-20} {"След. дата",-12} {"Примечания",-25}");
            Console.WriteLine(new string('-', 100));
            foreach (var v in vacs)
            {
                var next = v.NextDueDate?.ToString("dd.MM.yyyy") ?? "—";
                Console.WriteLine($"{v.VaccinationDate:dd.MM.yyyy,-12} {v.Vaccination?.VaccineName,-25} {v.BatchNumber,-12} {v.VeterinarianName,-20} {next,-12} {v.Notes,-25}");
            }
        }

        public void CreateMedicalRecord()
        {
            MenuHelper.PrintHeader("Создание медицинской карты");
            var animalId = ReadIntRequired("ID животного: ");

            var exists = _medicalService.GetMedicalRecordByAnimalId(animalId);
            if (exists != null)
            {
                MenuHelper.PrintError("Для этого животного карта уже существует.");
                return;
            }

            var record = new MedicalRecord
            {
                AnimalId = animalId,
                RecordNumber = ReadNonEmpty("Номер карты (например, MR-0001): "),
                BloodType = ReadOptional("Группа крови (необязательно): "),
                Allergies = ReadOptional("Аллергии (необязательно): "),
                ChronicConditions = ReadOptional("Хронические заболевания (необязательно): "),
                SpecialNeeds = ReadOptional("Особые нужды (необязательно): "),
                VeterinarianNotes = ReadOptional("Заметки ветеринара (необязательно): "),
                LastCheckupDate = ReadDateOptional("Последний осмотр (дд.мм.гггг, необязательно): ", null)
            };

            try
            {
                _medicalService.CreateMedicalRecord(record);
                MenuHelper.PrintSuccess("Медицинская карта создана.");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка создания карты: {ex.Message}");
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

            Console.WriteLine($"{ "Животное",-20} {"Вакцина",-25} {"Срок истёк",-12}");
            Console.WriteLine(new string('-', 65));
            foreach (var v in overdue)
            {
                var due = v.NextDueDate?.ToString("dd.MM.yyyy") ?? "—";
                Console.WriteLine($"{v.MedicalRecord?.Animal?.Name,-20} {v.Vaccination?.VaccineName,-25} {due,-12}");
            }
            MenuHelper.PrintInfo($"\nВсего просроченных: {overdue.Count}");
        }

        public void ListAllVaccinations()
        {
            MenuHelper.PrintHeader("Справочник вакцин");
            var vaccinations = _medicalService.GetAllVaccinations();
            if (!vaccinations.Any())
            {
                MenuHelper.PrintInfo("Нет зарегистрированных вакцин.");
                return;
            }

            Console.WriteLine($"{ "ID",-5} {"Название",-30} {"Производитель",-20} {"Срок (мес.)",-12}");
            Console.WriteLine(new string('-', 80));
            foreach (var v in vaccinations)
            {
                Console.WriteLine($"{v.VaccinationId,-5} {v.VaccineName,-30} {v.Manufacturer,-20} {v.ValidityPeriodMonths,-12}");
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

        #endregion
    }
}
