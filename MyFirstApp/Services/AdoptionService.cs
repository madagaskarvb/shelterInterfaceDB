using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;

namespace AnimalShelterCLI.Services
{
    public class AdoptionService
    {
        private readonly ShelterContext _context;

        public AdoptionService(ShelterContext context)
        {
            _context = context;
        }

        public List<Adoption> GetAllAdoptions()
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Include(a => a.Status)
                .ToList();
        }

        public Adoption GetAdoptionById(int id)
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Include(a => a.Status)
                .FirstOrDefault(a => a.AdoptionId == id);
        }

        public List<Adoption> GetAdoptionsByAdopter(int adopterId)
        {
            return _context.Adoptions
                .Include(a => a.Animal)
                .Include(a => a.Status)
                .Where(a => a.AdopterId == adopterId)
                .ToList();
        }

        public List<Adoption> GetAdoptionsByStatus(int statusId)
        {
            return _context.Adoptions
                .Include(a => a.Adopter)
                .Include(a => a.Animal)
                .Where(a => a.StatusId == statusId)
                .ToList();
        }

        public void CreateAdoption(Adoption adoption)
        {
            adoption.AdoptionDate = DateTime.Now;
            adoption.CreatedAt = DateTime.Now;
            _context.Adoptions.Add(adoption);
            _context.SaveChanges();
        }

        public void UpdateAdoption(Adoption adoption)
        {
            _context.Adoptions.Update(adoption);
            _context.SaveChanges();
        }

        public void DeleteAdoption(int id)
        {
            var adoption = _context.Adoptions.Find(id);
            if (adoption != null)
            {
                _context.Adoptions.Remove(adoption);
                _context.SaveChanges();
            }
        }

        public List<AdoptionStatus> GetAllStatuses()
        {
            return _context.AdoptionStatuses.ToList();
        }
    }
}
