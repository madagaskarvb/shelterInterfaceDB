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

        public bool RecordNumberExists(string recordNumber)
        {
            return _context.MedicalRecords.Any(mr => mr.RecordNumber == recordNumber);
        }
        
        public bool RecordNumberExistsForOther(string recordNumber, int animalId)
        {
            return _context.MedicalRecords.Any(mr => mr.RecordNumber == recordNumber && mr.AnimalId != animalId);
        }

        public HashSet<int> GetAnimalIdsWithMedicalRecords()
        {
            return _context.MedicalRecords.Select(mr => mr.AnimalId).ToHashSet();
        }

        public List<Animal> GetAnimalsWithoutMedicalRecord()
        {
            // Животные, у которых нет записи в medical_records
            var idsWith = _context.MedicalRecords.Select(mr => mr.AnimalId);
            return _context.Animals
                .Where(a => !idsWith.Contains(a.AnimalId))
                .OrderBy(a => a.AnimalId)
                .ToList();
        }

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
            if (RecordNumberExists(record.RecordNumber))
                throw new ArgumentException("Медицинская карта с таким номером уже существует.");

            record.CreatedDate = DateTime.UtcNow;
            record.CreatedAt = DateTime.UtcNow;
            record.UpdatedAt = DateTime.UtcNow;

            _context.MedicalRecords.Add(record);
            _context.SaveChanges();
        }

        public void UpdateMedicalRecord(MedicalRecord record)
        {
            if (RecordNumberExistsForOther(record.RecordNumber, record.AnimalId))
                throw new ArgumentException("Номер медицинской карты уже используется другим животным.");

            record.UpdatedAt = DateTime.UtcNow;
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
            vaccination.CreatedAt =  DateTime.UtcNow;
            _context.Vaccinations.Add(vaccination);
            _context.SaveChanges();
        }

        public void AddAnimalVaccination(AnimalVaccination animalVaccination)
        {
            animalVaccination.VaccinationDate = DateTime.UtcNow; 
            animalVaccination.CreatedAt = DateTime.UtcNow;
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
            // Используем локальную переменную, чтобы EF передал параметр как timestamp без часового пояса
            var now = DateTime.UtcNow;
            return _context.AnimalVaccinations
                .Include(av => av.Vaccination)
                .Include(av => av.MedicalRecord)
                    .ThenInclude(mr => mr.Animal)
                .Where(av => av.NextDueDate.HasValue && av.NextDueDate < now)
                .OrderBy(av => av.NextDueDate)
                .ToList();
        }

        public AnimalVaccination? GetVaccinationRecordById(int vaccinationRecordId)
        {
            return _context.AnimalVaccinations
                .Include(av => av.Vaccination)
                .Include(av => av.MedicalRecord)
                    .ThenInclude(mr => mr.Animal)
                .FirstOrDefault(av => av.VaccinationRecordId == vaccinationRecordId);
        }

        public void UpdateAnimalVaccination(AnimalVaccination record)
        {
            _context.AnimalVaccinations.Update(record);
            _context.SaveChanges();
        }

        public void DeleteAnimalVaccination(int vaccinationRecordId)
        {
            var rec = _context.AnimalVaccinations.Find(vaccinationRecordId);
            if (rec != null)
            {
                _context.AnimalVaccinations.Remove(rec);
                _context.SaveChanges();
            }
        }
    }
}
