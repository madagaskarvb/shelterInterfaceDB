using System;
using System.Collections.Generic;
using System.Linq;
using AnimalShelter.Interfaces;
using AnimalShelter.Models;

namespace AnimalShelter.Services
{
    public class AnimalService
    {
        private readonly IAnimalRepository _animalRepository;

        public AnimalService(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public void AddAnimal(string name, int age, string species, string breed, string gender, int statusId, string description)
        {
            var animal = new Animal
            {
                Name = name,
                Age = age,
                Species = species,
                Breed = breed,
                Gender = gender,
                StatusId = statusId,
                Description = description,
                DateAdmitted = DateTime.Now
            };

            _animalRepository.Add(animal);
            _animalRepository.SaveChanges();
            Console.WriteLine($"Животное {name} добавлено в приют!");
        }

        public void UpdateAnimal(int animalId, string name, int? age, string species, string breed, string gender, int? statusId, string description)
        {
            var animal = _animalRepository.GetById(animalId);
            if (animal == null)
            {
                Console.WriteLine("Животное не найдено!");
                return;
            }

            if (!string.IsNullOrEmpty(name)) animal.Name = name;
            if (age.HasValue) animal.Age = age.Value;
            if (!string.IsNullOrEmpty(species)) animal.Species = species;
            if (!string.IsNullOrEmpty(breed)) animal.Breed = breed;
            if (!string.IsNullOrEmpty(gender)) animal.Gender = gender;
            if (statusId.HasValue) animal.StatusId = statusId.Value;
            if (!string.IsNullOrEmpty(description)) animal.Description = description;

            _animalRepository.Update(animal);
            _animalRepository.SaveChanges();
            Console.WriteLine($"Данные животного {animal.Name} обновлены!");
        }

        public void DeleteAnimal(int animalId)
        {
            _animalRepository.Delete(animalId);
            _animalRepository.SaveChanges();
            Console.WriteLine("Животное удалено из системы!");
        }

        public Animal GetAnimalById(int animalId)
        {
            return _animalRepository.GetById(animalId);
        }

        public IEnumerable<Animal> GetAllAnimals()
        {
            return _animalRepository.GetAll();
        }

        public IEnumerable<Animal> GetAnimalsByStatus(int statusId)
        {
            return _animalRepository.GetAnimalsByStatus(statusId);
        }
    }
}
