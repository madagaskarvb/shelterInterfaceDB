using System;
using System.Collections.Generic;
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

        public void AddAnimal(Animal animal)
        {
            try
            {
                _animalRepository.Add(animal);
                _animalRepository.SaveChanges();
                Console.WriteLine($"Животное '{animal.Name}' успешно добавлено в систему!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении животного: {ex.Message}");
            }
        }

        public Animal GetAnimalById(int id)
        {
            return _animalRepository.GetById(id);
        }

        public IEnumerable<Animal> GetAllAnimals()
        {
            return _animalRepository.GetAll();
        }

        public IEnumerable<Animal> GetAnimalsByStatus(string status)
        {
            return _animalRepository.GetAnimalsByStatus(status);
        }

        public IEnumerable<Animal> GetAnimalsBySpecies(string species)
        {
            return _animalRepository.GetAnimalsBySpecies(species);
        }

        public Animal GetAnimalWithMedicalRecord(int animalId)
        {
            return _animalRepository.GetAnimalWithMedicalRecord(animalId);
        }

        public void UpdateAnimal(Animal animal)
        {
            try
            {
                _animalRepository.Update(animal);
                _animalRepository.SaveChanges();
                Console.WriteLine("Информация о животном успешно обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении: {ex.Message}");
            }
        }

        public void DeleteAnimal(int id)
        {
            try
            {
                _animalRepository.Delete(id);
                _animalRepository.SaveChanges();
                Console.WriteLine("Животное удалено из системы!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
            }
        }

        public void UpdateAnimalStatus(int animalId, string newStatus)
        {
            var animal = _animalRepository.GetById(animalId);
            if (animal != null)
            {
                animal.Status = newStatus;
                _animalRepository.Update(animal);
                _animalRepository.SaveChanges();
                Console.WriteLine($"Статус животного изменен на '{newStatus}'");
            }
        }
    }
}
