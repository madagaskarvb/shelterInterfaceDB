using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Data;
using AnimalShelter.Interfaces;
using AnimalShelter.Models;

namespace AnimalShelter.Repositories
{
    public class AdoptionRepository : Repository<Adoption>, IAdoptionRepository
    {
        public AdoptionRepository(AnimalShelterContext context) : base(context)
        {
        }

        public IEnumerable<Adoption> GetAdoptionsByStatus(string status)
        {
            return _dbSet
                .Include(a => a.Animal)
                .Include(a => a.Adopter)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.AdoptionDate)
                .ToList();
        }

        public IEnumerable<Adoption> GetAdoptionsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbSet
                .Include(a => a.Animal)
                .Include(a => a.Adopter)
                .Where(a => a.AdoptionDate >= startDate && a.AdoptionDate <= endDate)
                .OrderByDescending(a => a.AdoptionDate)
                .ToList();
        }

        public IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId)
        {
            return _dbSet
                .Include(a => a.Animal)
                .Where(a => a.AdopterId == adopterId)
                .OrderByDescending(a => a.AdoptionDate)
                .ToList();
        }
    }
}
