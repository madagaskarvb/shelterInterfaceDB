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

        public IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId)
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Include(a => a.AdoptionStatus)
                .Where(a => a.AdopterId == adopterId)
                .ToList();
        }

        public IEnumerable<Adoption> GetAdoptionsByStatus(int statusId)
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Include(a => a.AdoptionStatus)
                .Where(a => a.StatusId == statusId)
                .ToList();
        }

        public override Adoption GetById(int id)
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Include(a => a.AdoptionStatus)
                .FirstOrDefault(a => a.AdoptionId == id);
        }

        public override IEnumerable<Adoption> GetAll()
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Include(a => a.AdoptionStatus)
                .ToList();
        }
    }
}
