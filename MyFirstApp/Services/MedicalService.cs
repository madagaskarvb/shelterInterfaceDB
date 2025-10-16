using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;

namespace AnimalShelterCLI.Services
{
    public class MedicalService
    {
        private readonly ShelterContext _context;

        public MedicalService(ShelterContext context)
        {
            _context = context;
        }

        public MedicalRecord GetMedicalRecordByAnimalId(int animalId)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Animal)
                .Include(mr => mr.AnimalVaccinations)
                    .ThenInclude(av => av.Vaccination)
                .FirstOrDefault(mr => mr.AnimalId == animalId);
        }

        public void CreateMedicalRecord(MedicalRecord record)
        {
            record.CreatedDate = DateTime.Now;
            record.CreatedAt = DateTime.Now;
            record.UpdatedAt = DateTime.Now;
            _context.MedicalRecords.Add(record);
            _context.SaveChanges();
        }

        public void UpdateMedicalRecord(MedicalRecord record)
        {
            record.UpdatedAt = DateTime.Now;
            _context.MedicalRecords.Update(record);
            _context.SaveChanges();
        }

        public void DeleteMedicalRecord(int animalId)
        {
            var record = _context.MedicalRecords.Find(animalId);
            if (record != null)
            {
                _context.MedicalRecords.Remove(record);
                _context.SaveChanges();
            }
        }

        public List<Vaccination> GetAllVaccinations()
        {
            return _context.Vaccinations.ToList();
        }

        public void AddVaccination(Vaccination vaccination)
        {
            vaccination.CreatedAt = DateTime.Now;
            _context.Vaccinations.Add(vaccination);
            _context.SaveChanges();
        }

        public void AddAnimalVaccination(AnimalVaccination animalVaccination)
        {
            animalVaccination.VaccinationDate = DateTime.Now;
            animalVaccination.CreatedAt = DateTime.Now;
            _context.AnimalVaccinations.Add(animalVaccination);
            _context.SaveChanges();
        }

        public List<AnimalVaccination> GetVaccinationsByAnimal(int animalId)
        {
            return _context.AnimalVaccinations
                .Include(av => av.Vaccination)
                .Where(av => av.AnimalId == animalId)
                .ToList();
        }

        public List<AnimalVaccination> GetOverdueVaccinations()
        {
            return _context.AnimalVaccinations
                .Include(av => av.Vaccination)
                .Include(av => av.MedicalRecord)
                    .ThenInclude(mr => mr.Animal)
                .Where(av => av.NextDueDate.HasValue && av.NextDueDate < DateTime.Now)
                .ToList();
        }
    }
}
