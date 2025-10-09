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

        public IEnumerable<Animal> GetAnimalsByStatus(int statusId)
        {
            return _context.Animals
                .Include(a => a.AnimalStatus)
                .Include(a => a.MedicalRecord)
                .Where(a => a.StatusId == statusId)
                .ToList();
        }

        public override Animal GetById(int id)
        {
            return _context.Animals
                .Include(a => a.AnimalStatus)
                .Include(a => a.MedicalRecord)
                .FirstOrDefault(a => a.AnimalId == id);
        }

        public override IEnumerable<Animal> GetAll()
        {
            return _context.Animals
                .Include(a => a.AnimalStatus)
                .Include(a => a.MedicalRecord)
                .ToList();
        }
    }
}
