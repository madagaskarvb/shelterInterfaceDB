using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;

namespace AnimalShelterCLI.Services
{
    public class AdopterService
    {
        private readonly ShelterContext _context;

        public AdopterService(ShelterContext context)
        {
            _context = context;
        }

        public List<Adopter> GetAllAdopters()
        {
            return _context.Adopters
                .Include(a => a.Adoptions)
                .ToList();
        }

        public Adopter GetAdopterById(int id)
        {
            return _context.Adopters
                .Include(a => a.Adoptions)
                    .ThenInclude(ad => ad.Animal)
                .FirstOrDefault(a => a.AdopterId == id);
        }

        public List<Adopter> SearchAdopters(string searchTerm)
        {
            return _context.Adopters
                .Where(a => a.FirstName.Contains(searchTerm) || 
                           a.LastName.Contains(searchTerm) ||
                           a.Email.Contains(searchTerm))
                .ToList();
        }

        public void AddAdopter(Adopter adopter)
        {
            adopter.RegistrationDate = DateTime.Now;
            adopter.CreatedAt = DateTime.Now;
            _context.Adopters.Add(adopter);
            _context.SaveChanges();
        }

        public void UpdateAdopter(Adopter adopter)
        {
            _context.Adopters.Update(adopter);
            _context.SaveChanges();
        }

        public void DeleteAdopter(int id)
        {
            var adopter = _context.Adopters.Find(id);
            if (adopter != null)
            {
                _context.Adopters.Remove(adopter);
                _context.SaveChanges();
            }
        }
    }
}
