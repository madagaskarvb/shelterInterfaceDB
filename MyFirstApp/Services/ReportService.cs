using System;
using System.Collections.Generic;
using System.Linq;
using AnimalShelter.Data;
using AnimalShelter.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Services
{
    public class ReportService
    {
        private readonly AnimalShelterContext _context;

        public ReportService(AnimalShelterContext context)
        {
            _context = context;
        }

        // Выборка 1: Животные по статусу с сортировкой
        public List<Animal> GetAnimalsByStatusReport(string status)
        {
            return _context.Animals
                .Where(a => a.Status == status)
                .OrderBy(a => a.DateAdmitted)
                .ToList();
        }

        // Выборка 2: Усыновления по датам с фильтрацией
        public List<Adoption> GetAdoptionsByDateRangeReport(DateTime startDate, DateTime endDate, string status)
        {
            return _context.Adoptions
                .Include(a => a.Animal)
                .Include(a => a.Adopter)
                .Where(a => a.AdoptionDate >= startDate && 
                           a.AdoptionDate <= endDate && 
                           a.Status == status)
                .OrderByDescending(a => a.AdoptionDate)
                .ToList();
        }

        // Выборка 3: Животные по виду и возрасту
        public List<Animal> GetAnimalsBySpeciesAndAgeReport(string species, int minAge, int maxAge)
        {
            return _context.Animals
                .Include(a => a.MedicalRecord)
                .Where(a => a.Species == species && 
                           a.Age >= minAge && 
                           a.Age <= maxAge)
                .OrderBy(a => a.Age)
                .ThenBy(a => a.Name)
                .ToList();
        }

        // Выборка 4: Предстоящие вакцинации
        public List<AnimalVaccination> GetUpcomingVaccinations(int daysAhead)
        {
            var targetDate = DateTime.Now.AddDays(daysAhead);
            return _context.AnimalVaccinations
                .Include(av => av.MedicalRecord.Animal)
                .Include(av => av.Vaccination)
                .Where(av => av.NextDueDate <= targetDate && av.NextDueDate >= DateTime.Now)
                .OrderBy(av => av.NextDueDate)
                .ToList();
        }

        // Статистика по приюту
        public void DisplayShelterStatistics()
        {
            var totalAnimals = _context.Animals.Count();
            var inShelter = _context.Animals.Count(a => a.Status == "InShelter");
            var adopted = _context.Animals.Count(a => a.Status == "Adopted");
            var inTreatment = _context.Animals.Count(a => a.Status == "Treatment");

            Console.WriteLine("\n=== СТАТИСТИКА ПРИЮТА ===");
            Console.WriteLine($"Всего животных: {totalAnimals}");
            Console.WriteLine($"В приюте: {inShelter}");
            Console.WriteLine($"Усыновлено: {adopted}");
            Console.WriteLine($"На лечении: {inTreatment}");
        }
    }
}
