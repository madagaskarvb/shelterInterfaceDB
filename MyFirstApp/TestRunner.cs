using System;
using System.Diagnostics;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;
using AnimalShelterCLI.Services;

public class TestRunner
{
    private readonly ShelterContext _context;
    private int _totalTests = 0;
    private int _passedTests = 0;
    private int _failedTests = 0;
    private Stopwatch _stopwatch;

    public TestRunner(ShelterContext context)
    {
        _context = context;
        _stopwatch = new Stopwatch();
    }

    public void RunAllTests()
    {
        _stopwatch.Start();
        
        PrintHeader();
        
        try
        {
            TestAnimalCRUD();
            TestAdopterCRUD();
            TestAdoptionCRUD();
            TestMedicalCRUD();
            
            _stopwatch.Stop();
            PrintSummary(true);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            _stopwatch.Stop();
            PrintDatabaseError(ex);
            PrintSummary(false);
        }
        catch (Exception ex)
        {
            _stopwatch.Stop();
            PrintGeneralError(ex);
            PrintSummary(false);
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         🧪 СИСТЕМА ТЕСТИРОВАНИЯ БАЗЫ ДАННЫХ ПРИЮТА           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private void PrintTestGroupHeader(string groupName)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n┌─── {groupName} ───────────────────────────────────────");
        Console.ResetColor();
    }

    private void PrintTestStep(string operation, bool success)
    {
        _totalTests++;
        if (success)
        {
            _passedTests++;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ {operation}");
        }
        else
        {
            _failedTests++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ {operation}");
        }
        Console.ResetColor();
    }

    private void PrintSummary(bool allPassed)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     📊 ИТОГОВАЯ СТАТИСТИКА                    ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();
        
        Console.Write("║  Всего тестов:        ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{_totalTests,-40}");
        Console.ResetColor();
        Console.WriteLine("║");
        
        Console.Write("║  Успешно:             ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{_passedTests,-40}");
        Console.ResetColor();
        Console.WriteLine("║");
        
        Console.Write("║  Провалено:           ");
        Console.ForegroundColor = _failedTests > 0 ? ConsoleColor.Red : ConsoleColor.Gray;
        Console.Write($"{_failedTests,-40}");
        Console.ResetColor();
        Console.WriteLine("║");
        
        Console.Write("║  Процент успеха:      ");
        double successRate = _totalTests > 0 ? (_passedTests * 100.0 / _totalTests) : 0;
        Console.ForegroundColor = successRate == 100 ? ConsoleColor.Green : 
                                  successRate >= 75 ? ConsoleColor.Yellow : ConsoleColor.Red;
        Console.Write($"{successRate:F1}%{"",-36}");
        Console.ResetColor();
        Console.WriteLine("║");
        
        Console.Write("║  Время выполнения:    ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{_stopwatch.ElapsedMilliseconds}ms{"",-36}");
        Console.ResetColor();
        Console.WriteLine("║");
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║                         РЕЗУЛЬТАТ:                            ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();
        
        if (allPassed && _failedTests == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("║             ✓✓✓ ВСЕ ТЕСТЫ ПРОШЛИ УСПЕШНО! ✓✓✓               ║");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("║               ✗✗✗ ОБНАРУЖЕНЫ ОШИБКИ! ✗✗✗                    ║");
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private void PrintDatabaseError(Microsoft.EntityFrameworkCore.DbUpdateException ex)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   ⚠️  ОШИБКА БАЗЫ ДАННЫХ                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n📋 Сообщение: {ex.Message}");
        
        if (ex.InnerException != null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n🔍 Детали ошибки:");
            Console.WriteLine($"   {ex.InnerException.Message}");
            Console.WriteLine($"\n🏷️  Тип: {ex.InnerException.GetType().Name}");
        }
        Console.ResetColor();
    }

    private void PrintGeneralError(Exception ex)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                      ⚠️  КРИТИЧЕСКАЯ ОШИБКА                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n📋 Сообщение: {ex.Message}");
        Console.WriteLine($"🏷️  Тип: {ex.GetType().Name}");
        Console.ResetColor();
    }

    private void TestAnimalCRUD()
    {
        PrintTestGroupHeader("🐾 ТЕСТИРОВАНИЕ ЖИВОТНЫХ (CRUD)");
        
        var service = new AnimalService(_context);
        
        // CREATE
        var animal = new Animal
        {
            Name = "TEST_ANIMAL",
            Species = "Dog",
            Breed = "Labrador",
            Age = 2,
            Gender = "Male",
            DateAdmitted = DateTime.UtcNow,
            StatusId = 1,
            Description = "Тест для CRUD животных",
            CreatedAt = DateTime.UtcNow
        };
        
        service.AddAnimal(animal);
        var fetched = service.GetAnimalById(animal.AnimalId);
        PrintTestStep($"CREATE: Добавление животного '{animal.Name}' (ID: {animal.AnimalId})", 
                     fetched != null && fetched.Name == animal.Name);
        
        // READ
        PrintTestStep($"READ: Получение данных о животном (ID: {animal.AnimalId})", 
                     fetched != null);
        
        // UPDATE
        animal.Name = "UPDATED_ANIMAL";
        service.UpdateAnimal(animal);
        var updated = service.GetAnimalById(animal.AnimalId);
        PrintTestStep($"UPDATE: Изменение имени на '{animal.Name}'", 
                     updated.Name == "UPDATED_ANIMAL");
        
        // DELETE
        service.DeleteAnimal(animal.AnimalId);
        var deleted = service.GetAnimalById(animal.AnimalId);
        PrintTestStep($"DELETE: Удаление животного (ID: {animal.AnimalId})", 
                     deleted == null);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("└─────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    private void TestAdopterCRUD()
    {
        PrintTestGroupHeader("👤 ТЕСТИРОВАНИЕ УСЫНОВИТЕЛЕЙ (CRUD)");
        
        var service = new AdopterService(_context);
        
        // CREATE
        var adopter = new Adopter
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            Email = "ivan@test.com",
            Phone = "+79161234567",
            Address = "Test address",
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        service.AddAdopter(adopter);
        var fetched = service.GetAdopterById(adopter.AdopterId);
        PrintTestStep($"CREATE: Регистрация усыновителя '{adopter.FirstName} {adopter.LastName}' (ID: {adopter.AdopterId})", 
                     fetched != null && fetched.Email == adopter.Email);
        
        // READ
        PrintTestStep($"READ: Получение данных усыновителя (Email: {adopter.Email})", 
                     fetched != null);
        
        // UPDATE
        adopter.LastName = "Updated";
        service.UpdateAdopter(adopter);
        var updated = service.GetAdopterById(adopter.AdopterId);
        PrintTestStep($"UPDATE: Изменение фамилии на '{adopter.LastName}'", 
                     updated.LastName == "Updated");
        
        // DELETE
        service.DeleteAdopter(adopter.AdopterId);
        var deleted = service.GetAdopterById(adopter.AdopterId);
        PrintTestStep($"DELETE: Удаление усыновителя (ID: {adopter.AdopterId})", 
                     deleted == null);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("└─────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    private void TestAdoptionCRUD()
    {
        PrintTestGroupHeader("🏠 ТЕСТИРОВАНИЕ УСЫНОВЛЕНИЙ (CRUD)");
        
        var animalService = new AnimalService(_context);
        var adopterService = new AdopterService(_context);
        var adoptionService = new AdoptionService(_context);
        
        // Подготовка данных
        var animal = new Animal
        {
            Name = "ADOPT_TEST",
            Species = "Cat",
            Breed = "Siamese",
            Age = 3,
            Gender = "Female",
            DateAdmitted = DateTime.UtcNow,
            StatusId = 1,
            Description = "Тестовое животное для усыновления",
            CreatedAt = DateTime.UtcNow
        };
        animalService.AddAnimal(animal);
        
        var adopter = new Adopter
        {
            FirstName = "Petr",
            LastName = "Petrov",
            Email = "petr@test.com",
            Phone = "+79161234568",
            Address = "Test address",
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        adopterService.AddAdopter(adopter);
        
        PrintTestStep($"SETUP: Создание тестовых данных (Animal ID: {animal.AnimalId}, Adopter ID: {adopter.AdopterId})", true);
        
        // CREATE
        var adoption = new Adoption
        {
            AdopterId = adopter.AdopterId,
            AnimalId = animal.AnimalId,
            AdoptionDate = DateTime.UtcNow,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };
        adoptionService.CreateAdoption(adoption);
        var fetched = adoptionService.GetAdoptionById(adoption.AdoptionId);
        PrintTestStep($"CREATE: Создание записи об усыновлении (ID: {adoption.AdoptionId})", 
                     fetched != null && fetched.AdopterId == adopter.AdopterId);
        
        // READ
        PrintTestStep($"READ: Получение данных об усыновлении (ID: {adoption.AdoptionId})", 
                     fetched != null);
        
        // UPDATE
        fetched.Notes = "Updated status note";
        adoptionService.UpdateAdoption(fetched);
        var updated = adoptionService.GetAdoptionById(adoption.AdoptionId);
        PrintTestStep($"UPDATE: Добавление заметки 'Updated status note'", 
                     updated.Notes == "Updated status note");
        
        // DELETE
        adoptionService.DeleteAdoption(adoption.AdoptionId);
        var deleted = adoptionService.GetAdoptionById(adoption.AdoptionId);
        PrintTestStep($"DELETE: Удаление записи об усыновлении (ID: {adoption.AdoptionId})", 
                     deleted == null);
        
        // Cleanup
        animalService.DeleteAnimal(animal.AnimalId);
        adopterService.DeleteAdopter(adopter.AdopterId);
        PrintTestStep("CLEANUP: Очистка тестовых данных", true);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("└─────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    private void TestMedicalCRUD()
    {
        PrintTestGroupHeader("🏥 ТЕСТИРОВАНИЕ МЕДИЦИНСКИХ ЗАПИСЕЙ (CRUD)");
        
        var animalService = new AnimalService(_context);
        var medicalService = new MedicalService(_context);
        
        // Подготовка данных
        var animal = new Animal
        {
            Name = "MED_TEST",
            Species = "Mouse",
            Breed = "Unknown",
            Age = 1,
            Gender = "Male",
            DateAdmitted = DateTime.UtcNow,
            StatusId = 1,
            Description = "Медицинский тест",
            CreatedAt = DateTime.UtcNow
        };
        animalService.AddAnimal(animal);
        PrintTestStep($"SETUP: Создание тестового животного (ID: {animal.AnimalId})", true);
        
        // CREATE
        var record = new MedicalRecord
        {
            AnimalId = animal.AnimalId,
            RecordNumber = "M-TEST-001",
            CreatedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        medicalService.CreateMedicalRecord(record);
        var fetched = medicalService.GetMedicalRecordByAnimalId(animal.AnimalId);
        PrintTestStep($"CREATE: Создание медицинской карты (№ {record.RecordNumber})", 
                     fetched != null && fetched.RecordNumber == record.RecordNumber);
        
        // READ
        PrintTestStep($"READ: Получение медицинской карты (Animal ID: {animal.AnimalId})", 
                     fetched != null);
        
        // UPDATE
        fetched.Allergies = "Updated allergy";
        medicalService.UpdateMedicalRecord(fetched);
        var updated = medicalService.GetMedicalRecordByAnimalId(animal.AnimalId);
        PrintTestStep($"UPDATE: Добавление информации об аллергии", 
                     updated.Allergies == "Updated allergy");
        
        // DELETE
        medicalService.DeleteMedicalRecord(animal.AnimalId);
        var deleted = medicalService.GetMedicalRecordByAnimalId(animal.AnimalId);
        PrintTestStep($"DELETE: Удаление медицинской карты (Animal ID: {animal.AnimalId})", 
                     deleted == null);
        
        // Cleanup
        animalService.DeleteAnimal(animal.AnimalId);
        PrintTestStep("CLEANUP: Удаление тестового животного", true);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("└─────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }
}
