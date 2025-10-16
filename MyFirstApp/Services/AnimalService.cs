using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;

namespace AnimalShelterCLI.Services
{
    public class AnimalService
    {
        private readonly ShelterContext _context;

        public AnimalService(ShelterContext context)
        {
            _context = context;
        }

        public List<Animal> GetAllAnimals()
        {
            return _context.Animals
                .Include(a => a.Status)
                .ToList();
        }

        public Animal GetAnimalById(int id)
        {
            return _context.Animals
                .Include(a => a.Status)
                .Include(a => a.MedicalRecord)
                .FirstOrDefault(a => a.AnimalId == id);
        }

        public List<Animal> SearchAnimals(string searchTerm)
        {
            return _context.Animals
                .Include(a => a.Status)
                .Where(a => a.Name.Contains(searchTerm) || 
                           a.Species.Contains(searchTerm) || 
                           a.Breed.Contains(searchTerm))
                .ToList();
        }

        public List<Animal> GetAnimalsByStatus(int statusId)
        {
            return _context.Animals
                .Include(a => a.Status)
                .Where(a => a.StatusId == statusId)
                .ToList();
        }

        public void AddAnimal(Animal animal)
        {
            animal.DateAdmitted = DateTime.UtcNow;
            animal.CreatedAt = DateTime.UtcNow;  
            _context.Animals.Add(animal);
            _context.SaveChanges();
        }

        public void UpdateAnimal(Animal animal)
        {
            _context.Animals.Update(animal);
            _context.SaveChanges();
        }

        public void DeleteAnimal(int id)
        {
            var animal = _context.Animals.Find(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                _context.SaveChanges();
            }
        }

        public List<AnimalStatus> GetAllStatuses()
        {
            return _context.AnimalStatuses.ToList();
        }
    }
}
