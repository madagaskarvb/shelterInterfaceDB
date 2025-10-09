using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Interfaces;
using AnimalShelter.Models;

namespace AnimalShelter.Repositories
{
    public class AnimalRepository : Repository<Animal>, IAnimalRepository
    {
        public AnimalRepository(AnimalShelterContext context) : base(context)
        {
        }

        public IEnumerable<Animal> GetAnimalsByStatus(string status)
        {
            return _dbSet
                .Where(a => a.Status == status)
                .OrderBy(a => a.DateAdmitted)
                .ToList();
        }

        public IEnumerable<Animal> GetAnimalsBySpecies(string species)
        {
            return _dbSet
                .Where(a => a.Species == species)
                .OrderBy(a => a.Name)
                .ToList();
        }

        public Animal GetAnimalWithMedicalRecord(int animalId)
        {
            return _dbSet
                .Include(a => a.MedicalRecord)
                .FirstOrDefault(a => a.AnimalId == animalId);
        }
    }
}
