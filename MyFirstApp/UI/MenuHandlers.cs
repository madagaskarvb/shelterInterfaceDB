using System;
using System.Linq;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;
using AnimalShelterCLI.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

            Console.WriteLine($"{"ID",-5} {"Имя",-20} {"Вид",-15} {"Порода",-15} {"Возраст",-8} {"Статус",-15}");
            Console.WriteLine(new string('-', 85));

            foreach (var animal in animals)
            {
                Console.WriteLine($"{animal.AnimalId,-5} {animal.Name,-20} {animal.Species,-15} " +
                                $"{animal.Breed,-15} {animal.Age,-8} {animal.Status?.StatusName,-15}");
            }
        }

        public void AddAnimal()
    {
        MenuHelper.PrintHeader("Добавление нового животного");

        try
        {
            var animal = new Animal();

            Console.Write("Имя: ");
            animal.Name = Console.ReadLine();

            Console.Write("Вид (собака, кошка, и т.д.): ");
            animal.Species = Console.ReadLine();

            Console.Write("Порода: ");
            animal.Breed = Console.ReadLine();

            Console.Write("Возраст (лет): ");
            if (int.TryParse(Console.ReadLine(), out int age))
                animal.Age = age;

            // ИСПРАВЛЕНО: Валидация и нормализация пола
            Console.WriteLine("\nВыберите пол:");
            Console.WriteLine("1. Male (Мужской)");
            Console.WriteLine("2. Female (Женский)");
            Console.WriteLine("3. Unknown (Неизвестно)");
            Console.Write("Ваш выбор (1-3): ");

            var genderChoice = Console.ReadLine();
            animal.Gender = genderChoice switch
            {
                "1" => "Male",
                "2" => "Female",
                "3" => "Unknown",
                _ => "Unknown" // По умолчанию
            };

            Console.Write("Описание: ");
            animal.Description = Console.ReadLine();

            // Выбор статуса
            var statuses = _animalService.GetAllStatuses();

            if (!statuses.Any())
            {
                MenuHelper.PrintError("В базе данных нет доступных статусов! Сначала добавьте статусы.");
                return;
            }

            Console.WriteLine("\nДоступные статусы:");
            foreach (var status in statuses)
            {
                Console.WriteLine($"{status.StatusId}. {status.StatusName}");
            }
            Console.Write("Выберите ID статуса: ");

            if (!int.TryParse(Console.ReadLine(), out int statusId))
            {
                MenuHelper.PrintError("Неверный ID статуса");
                return;
            }

            animal.StatusId = statusId;

            _animalService.AddAnimal(animal);
            MenuHelper.PrintSuccess($"Животное '{animal.Name}' успешно добавлено!");
        }
        catch (DbUpdateException dbEx)
        {
            // Получаем детали ошибки базы данных
            var innerException = dbEx.InnerException;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=== ДЕТАЛИ ОШИБКИ БД ===");
            Console.WriteLine($"Сообщение: {dbEx.Message}");

            if (innerException != null)
            {
                Console.WriteLine($"\nВнутреннее исключение: {innerException.GetType().Name}");
                Console.WriteLine($"Сообщение БД: {innerException.Message}");

                // Для PostgreSQL
                if (innerException is Npgsql.PostgresException pgEx)
                {
                    Console.WriteLine($"SQL State: {pgEx.SqlState}");
                    Console.WriteLine($"Constraint: {pgEx.ConstraintName}");
                    Console.WriteLine($"Table: {pgEx.TableName}");
                    Console.WriteLine($"Detail: {pgEx.Detail}");
                }
            }
            Console.ResetColor();

            MenuHelper.PrintError("Ошибка при добавлении животного. Проверьте данные выше.");
        }
        catch (Exception ex)
        {
            MenuHelper.PrintError($"Ошибка при добавлении: {ex.Message}");
            Console.WriteLine($"\nТип ошибки: {ex.GetType().Name}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

        public void UpdateAnimal()
        {
            MenuHelper.PrintHeader("Обновление информации о животном");

            Console.Write("Введите ID животного: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено");
                return;
            }

            Console.WriteLine($"\nТекущие данные: {animal.Name}, {animal.Species}, возраст {animal.Age}");
            Console.WriteLine("\nОставьте поле пустым, чтобы не изменять значение");

            Console.Write($"Имя [{animal.Name}]: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) animal.Name = name;

            Console.Write($"Вид [{animal.Species}]: ");
            var species = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(species)) animal.Species = species;

            Console.Write($"Порода [{animal.Breed}]: ");
            var breed = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(breed)) animal.Breed = breed;

            Console.Write($"Возраст [{animal.Age}]: ");
            if (int.TryParse(Console.ReadLine(), out int age)) animal.Age = age;

            var statuses = _animalService.GetAllStatuses();
            Console.WriteLine("\nДоступные статусы:");
            foreach (var status in statuses)
            {
                Console.WriteLine($"{status.StatusId}. {status.StatusName}");
            }
            Console.Write($"Статус [{animal.StatusId}]: ");
            if (int.TryParse(Console.ReadLine(), out int statusId)) animal.StatusId = statusId;

            _animalService.UpdateAnimal(animal);
            MenuHelper.PrintSuccess("Данные успешно обновлены!");
        }

        public void DeleteAnimal()
        {
            MenuHelper.PrintHeader("Удаление животного");

            Console.Write("Введите ID животного: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено");
                return;
            }

            Console.WriteLine($"\nВы уверены, что хотите удалить: {animal.Name}? (да/нет)");
            var confirm = Console.ReadLine()?.ToLower();

            if (confirm == "да" || confirm == "yes")
            {
                _animalService.DeleteAnimal(id);
                MenuHelper.PrintSuccess("Животное удалено");
            }
            else
            {
                MenuHelper.PrintInfo("Удаление отменено");
            }
        }

        public void SearchAnimals()
        {
            MenuHelper.PrintHeader("Поиск животных");

            Console.Write("Введите поисковый запрос (имя, вид, порода): ");
            var searchTerm = Console.ReadLine();

            var animals = _animalService.SearchAnimals(searchTerm);

            if (!animals.Any())
            {
                MenuHelper.PrintInfo("Животные не найдены");
                return;
            }

            Console.WriteLine($"\nНайдено животных: {animals.Count}");
            Console.WriteLine($"{"ID",-5} {"Имя",-20} {"Вид",-15} {"Порода",-15}");
            Console.WriteLine(new string('-', 60));

            foreach (var animal in animals)
            {
                Console.WriteLine($"{animal.AnimalId,-5} {animal.Name,-20} {animal.Species,-15} {animal.Breed,-15}");
            }
        }

        public void ViewAnimalDetails()
        {
            MenuHelper.PrintHeader("Детальная информация о животном");

            Console.Write("Введите ID животного: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                MenuHelper.PrintError("Животное не найдено");
                return;
            }

            Console.WriteLine($"\n{"Параметр",-25} {"Значение"}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"ID:",-25} {animal.AnimalId}");
            Console.WriteLine($"{"Имя:",-25} {animal.Name}");
            Console.WriteLine($"{"Вид:",-25} {animal.Species}");
            Console.WriteLine($"{"Порода:",-25} {animal.Breed}");
            Console.WriteLine($"{"Возраст:",-25} {animal.Age}");
            Console.WriteLine($"{"Пол:",-25} {animal.Gender}");
            Console.WriteLine($"{"Статус:",-25} {animal.Status?.StatusName}");
            Console.WriteLine($"{"Дата поступления:",-25} {animal.DateAdmitted:dd.MM.yyyy}");
            Console.WriteLine($"{"Описание:",-25} {animal.Description}");

            if (animal.MedicalRecord != null)
            {
                Console.WriteLine($"\n{"Медицинская карта:",-25} ID {animal.MedicalRecord.RecordNumber}");
            }
        }

        #endregion

        #region Adopter Management

        public void ListAllAdopters()
        {
            MenuHelper.PrintHeader("Список усыновителей");
            var adopters = _adopterService.GetAllAdopters();

            if (!adopters.Any())
            {
                MenuHelper.PrintInfo("Нет зарегистрированных усыновителей");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Имя",-20} {"Фамилия",-20} {"Email",-30} {"Телефон",-15}");
            Console.WriteLine(new string('-', 95));

            foreach (var adopter in adopters)
            {
                Console.WriteLine($"{adopter.AdopterId,-5} {adopter.FirstName,-20} {adopter.LastName,-20} " +
                                $"{adopter.Email,-30} {adopter.Phone,-15}");
            }
        }

        public void AddAdopter()
        {
            MenuHelper.PrintHeader("Регистрация нового усыновителя");

            try
            {
                var adopter = new Adopter();

                Console.Write("Имя: ");
                adopter.FirstName = Console.ReadLine();

                Console.Write("Фамилия: ");
                adopter.LastName = Console.ReadLine();

                Console.Write("Email: ");
                adopter.Email = Console.ReadLine();

                Console.Write("Телефон: ");
                adopter.Phone = Console.ReadLine();

                Console.Write("Адрес: ");
                adopter.Address = Console.ReadLine();

                _adopterService.AddAdopter(adopter);
                MenuHelper.PrintSuccess("Усыновитель успешно зарегистрирован!");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка при регистрации: {ex.Message}");
            }
        }

        public void UpdateAdopter()
        {
            MenuHelper.PrintHeader("Обновление данных усыновителя");

            Console.Write("Введите ID усыновителя: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var adopter = _adopterService.GetAdopterById(id);
            if (adopter == null)
            {
                MenuHelper.PrintError("Усыновитель не найден");
                return;
            }

            Console.WriteLine($"\nТекущие данные: {adopter.FirstName} {adopter.LastName}");
            Console.WriteLine("Оставьте поле пустым, чтобы не изменять значение\n");

            Console.Write($"Имя [{adopter.FirstName}]: ");
            var firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName)) adopter.FirstName = firstName;

            Console.Write($"Фамилия [{adopter.LastName}]: ");
            var lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName)) adopter.LastName = lastName;

            Console.Write($"Email [{adopter.Email}]: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) adopter.Email = email;

            Console.Write($"Телефон [{adopter.Phone}]: ");
            var phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone)) adopter.Phone = phone;

            Console.Write($"Адрес [{adopter.Address}]: ");
            var address = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(address)) adopter.Address = address;

            _adopterService.UpdateAdopter(adopter);
            MenuHelper.PrintSuccess("Данные успешно обновлены!");
        }

        public void ViewAdopterDetails()
        {
            MenuHelper.PrintHeader("Информация об усыновителе");

            Console.Write("Введите ID усыновителя: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var adopter = _adopterService.GetAdopterById(id);
            if (adopter == null)
            {
                MenuHelper.PrintError("Усыновитель не найден");
                return;
            }

            Console.WriteLine($"\n{"Параметр",-25} {"Значение"}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"ID:",-25} {adopter.AdopterId}");
            Console.WriteLine($"{"Имя:",-25} {adopter.FirstName}");
            Console.WriteLine($"{"Фамилия:",-25} {adopter.LastName}");
            Console.WriteLine($"{"Email:",-25} {adopter.Email}");
            Console.WriteLine($"{"Телефон:",-25} {adopter.Phone}");
            Console.WriteLine($"{"Адрес:",-25} {adopter.Address}");
            Console.WriteLine($"{"Дата регистрации:",-25} {adopter.RegistrationDate:dd.MM.yyyy}");

            if (adopter.Adoptions != null && adopter.Adoptions.Any())
            {
                Console.WriteLine($"\n{"Усыновленные животные:",-25} {adopter.Adoptions.Count}");
                foreach (var adoption in adopter.Adoptions)
                {
                    Console.WriteLine($"  - {adoption.Animal?.Name} ({adoption.AdoptionDate:dd.MM.yyyy})");
                }
            }
        }

        #endregion

        #region Adoption Management

        public void ListAllAdoptions()
        {
            MenuHelper.PrintHeader("Список усыновлений");
            var adoptions = _adoptionService.GetAllAdoptions();

            if (!adoptions.Any())
            {
                MenuHelper.PrintInfo("Нет записей об усыновлениях");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Животное",-20} {"Усыновитель",-25} {"Дата",-12} {"Статус",-15}");
            Console.WriteLine(new string('-', 85));

            foreach (var adoption in adoptions)
            {
                var adopterName = $"{adoption.Adopter?.FirstName} {adoption.Adopter?.LastName}";
                Console.WriteLine($"{adoption.AdoptionId,-5} {adoption.Animal?.Name,-20} {adopterName,-25} " +
                                $"{adoption.AdoptionDate:dd.MM.yyyy,-12} {adoption.Status?.StatusName,-15}");
            }
        }

        public void CreateAdoption()
        {
            MenuHelper.PrintHeader("Создание записи об усыновлении");

            try
            {
                var adoption = new Adoption();

                Console.Write("ID животного: ");
                adoption.AnimalId = int.Parse(Console.ReadLine());

                Console.Write("ID усыновителя: ");
                adoption.AdopterId = int.Parse(Console.ReadLine());

                var statuses = _adoptionService.GetAllStatuses();
                Console.WriteLine("\nДоступные статусы:");
                foreach (var status in statuses)
                {
                    Console.WriteLine($"{status.StatusId}. {status.StatusName}");
                }
                Console.Write("Выберите ID статуса: ");
                adoption.StatusId = int.Parse(Console.ReadLine());

                Console.Write("Примечания: ");
                adoption.Notes = Console.ReadLine();

                _adoptionService.CreateAdoption(adoption);
                MenuHelper.PrintSuccess("Запись об усыновлении создана!");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        public void UpdateAdoption()
        {
            MenuHelper.PrintHeader("Обновление статуса усыновления");

            Console.Write("Введите ID усыновления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var adoption = _adoptionService.GetAdoptionById(id);
            if (adoption == null)
            {
                MenuHelper.PrintError("Запись не найдена");
                return;
            }

            Console.WriteLine($"\nТекущий статус: {adoption.Status?.StatusName}");

            var statuses = _adoptionService.GetAllStatuses();
            Console.WriteLine("\nДоступные статусы:");
            foreach (var status in statuses)
            {
                Console.WriteLine($"{status.StatusId}. {status.StatusName}");
            }
            Console.Write("Выберите новый статус: ");
            if (int.TryParse(Console.ReadLine(), out int statusId))
            {
                adoption.StatusId = statusId;
            }

            Console.Write("Обновить примечания? (да/нет): ");
            if (Console.ReadLine()?.ToLower() == "да")
            {
                Console.Write("Новые примечания: ");
                adoption.Notes = Console.ReadLine();
            }

            _adoptionService.UpdateAdoption(adoption);
            MenuHelper.PrintSuccess("Статус усыновления обновлен!");
        }

        #endregion

        #region Medical Management

        public void ViewMedicalRecord()
        {
            MenuHelper.PrintHeader("Просмотр медицинской карты");

            Console.Write("Введите ID животного: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                MenuHelper.PrintError("Неверный ID");
                return;
            }

            var record = _medicalService.GetMedicalRecordByAnimalId(animalId);
            if (record == null)
            {
                MenuHelper.PrintInfo("Медицинская карта не найдена");
                return;
            }

            Console.WriteLine($"\n{"Параметр",-25} {"Значение"}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"Номер карты:",-25} {record.RecordNumber}");
            Console.WriteLine($"{"Животное:",-25} {record.Animal?.Name}");
            Console.WriteLine($"{"Группа крови:",-25} {record.BloodType}");
            Console.WriteLine($"{"Аллергии:",-25} {record.Allergies}");
            Console.WriteLine($"{"Хрон. заболевания:",-25} {record.ChronicConditions}");
            Console.WriteLine($"{"Особые потребности:",-25} {record.SpecialNeeds}");
            Console.WriteLine($"{"Последний осмотр:",-25} {record.LastCheckupDate:dd.MM.yyyy}");
            Console.WriteLine($"{"Заметки ветеринара:",-25} {record.VeterinarianNotes}");

            if (record.AnimalVaccinations != null && record.AnimalVaccinations.Any())
            {
                Console.WriteLine("\nВакцинации:");
                foreach (var vac in record.AnimalVaccinations)
                {
                    Console.WriteLine($"  - {vac.Vaccination?.VaccineName} " +
                                    $"(дата: {vac.VaccinationDate:dd.MM.yyyy}, " +
                                    $"след.: {vac.NextDueDate:dd.MM.yyyy})");
                }
            }
        }

        public void CreateMedicalRecord()
        {
            MenuHelper.PrintHeader("Создание медицинской карты");

            try
            {
                var record = new MedicalRecord();

                Console.Write("ID животного: ");
                record.AnimalId = int.Parse(Console.ReadLine());

                Console.Write("Номер карты: ");
                record.RecordNumber = Console.ReadLine();

                Console.Write("Группа крови: ");
                record.BloodType = Console.ReadLine();

                Console.Write("Аллергии: ");
                record.Allergies = Console.ReadLine();

                Console.Write("Хронические заболевания: ");
                record.ChronicConditions = Console.ReadLine();

                Console.Write("Особые потребности: ");
                record.SpecialNeeds = Console.ReadLine();

                Console.Write("Заметки ветеринара: ");
                record.VeterinarianNotes = Console.ReadLine();

                _medicalService.CreateMedicalRecord(record);
                MenuHelper.PrintSuccess("Медицинская карта создана!");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        public void AddVaccinationRecord()
        {
            MenuHelper.PrintHeader("Добавление записи о вакцинации");

            try
            {
                var vacRecord = new AnimalVaccination();

                Console.Write("ID животного: ");
                vacRecord.AnimalId = int.Parse(Console.ReadLine());

                var vaccinations = _medicalService.GetAllVaccinations();
                Console.WriteLine("\nДоступные вакцины:");
                foreach (var vac in vaccinations)
                {
                    Console.WriteLine($"{vac.VaccinationId}. {vac.VaccineName}");
                }
                Console.Write("Выберите ID вакцины: ");
                vacRecord.VaccinationId = int.Parse(Console.ReadLine());

                Console.Write("Номер партии: ");
                vacRecord.BatchNumber = Console.ReadLine();

                Console.Write("Имя ветеринара: ");
                vacRecord.VeterinarianName = Console.ReadLine();

                Console.Write("Дата следующей вакцинации (дд.мм.гггг): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime nextDate))
                {
                    vacRecord.NextDueDate = nextDate;
                }

                Console.Write("Примечания: ");
                vacRecord.Notes = Console.ReadLine();

                _medicalService.AddAnimalVaccination(vacRecord);
                MenuHelper.PrintSuccess("Запись о вакцинации добавлена!");
            }
            catch (Exception ex)
            {
                MenuHelper.PrintError($"Ошибка: {ex.Message}");
            }
        }

        public void ViewOverdueVaccinations()
        {
            MenuHelper.PrintHeader("Просроченные вакцинации");

            var overdueVacs = _medicalService.GetOverdueVaccinations();

            if (!overdueVacs.Any())
            {
                MenuHelper.PrintSuccess("Нет просроченных вакцинаций!");
                return;
            }

            Console.WriteLine($"{"Животное",-20} {"Вакцина",-25} {"Срок истек",-15}");
            Console.WriteLine(new string('-', 65));

            foreach (var vac in overdueVacs)
            {
                Console.WriteLine($"{vac.MedicalRecord?.Animal?.Name,-20} " +
                                $"{vac.Vaccination?.VaccineName,-25} " +
                                $"{vac.NextDueDate:dd.MM.yyyy,-15}");
            }

            MenuHelper.PrintInfo($"\nВсего просроченных: {overdueVacs.Count}");
        }

        public void ListAllVaccinations()
        {
            MenuHelper.PrintHeader("Список вакцин");

            var vaccinations = _medicalService.GetAllVaccinations();

            if (!vaccinations.Any())
            {
                MenuHelper.PrintInfo("Нет зарегистрированных вакцин");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Название",-30} {"Производитель",-20} {"Срок действия (мес.)",-20}");
            Console.WriteLine(new string('-', 80));

            foreach (var vac in vaccinations)
            {
                Console.WriteLine($"{vac.VaccinationId,-5} {vac.VaccineName,-30} " +
                                $"{vac.Manufacturer,-20} {vac.ValidityPeriodMonths,-20}");
            }
        }

        public void AddVaccination()
        {
            MenuHelper.PrintHeader("Добавление новой вакцины");

            try
            {
                var vaccination = new Vaccination();

                Console.Write("Название вакцины: ");
                vaccination.VaccineName = Console.ReadLine();

                Console.Write("Описание: ");
                vaccination.Description = Console.ReadLine();

                Console.Write("Производитель: ");
                vaccination.Manufacturer = Console.ReadLine();

                Console.Write("Срок действия (месяцев): ");
                if (int.TryParse(Console.ReadLine(), out int validity))
                {
                    vaccination.ValidityPeriodMonths = validity;
                }

                _medicalService.AddVaccination(vaccination);
                MenuHelper.PrintSuccess("Вакцина добавлена в справочник!");
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
            foreach (var status in statuses)
            {
                var count = animals.Count(a => a.StatusId == status.StatusId);
                Console.WriteLine($"  {status.StatusName,-20}: {count}");
            }

            Console.WriteLine("\nРаспределение по видам:");
            var speciesGroups = animals.GroupBy(a => a.Species);
            foreach (var group in speciesGroups)
            {
                Console.WriteLine($"  {group.Key,-20}: {group.Count()}");
            }

            var avgAge = animals.Where(a => a.Age.HasValue).Average(a => a.Age.Value);
            Console.WriteLine($"\nСредний возраст: {avgAge:F1} лет");
        }

        public void GenerateAdoptionReport()
        {
            MenuHelper.PrintHeader("Отчет по усыновлениям");

            var adoptions = _adoptionService.GetAllAdoptions();
            var statuses = _adoptionService.GetAllStatuses();

            Console.WriteLine($"Всего записей об усыновлениях: {adoptions.Count}\n");

            Console.WriteLine("Распределение по статусам:");
            foreach (var status in statuses)
            {
                var count = adoptions.Count(a => a.StatusId == status.StatusId);
                Console.WriteLine($"  {status.StatusName,-20}: {count}");
            }

            var thisMonth = adoptions.Count(a => a.AdoptionDate.Month == DateTime.Now.Month &&
                                                 a.AdoptionDate.Year == DateTime.Now.Year);
            Console.WriteLine($"\nУсыновлений в текущем месяце: {thisMonth}");

            var thisYear = adoptions.Count(a => a.AdoptionDate.Year == DateTime.Now.Year);
            Console.WriteLine($"Усыновлений в текущем году: {thisYear}");
        }

        #endregion
    }
}
