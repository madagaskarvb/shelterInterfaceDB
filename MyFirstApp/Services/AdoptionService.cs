using System;
using System.Collections.Generic;
using System.Linq;
using AnimalShelter.Interfaces;
using AnimalShelter.Models;

namespace AnimalShelter.Services
{
    public class AdoptionService
    {
        private readonly IAdoptionRepository _adoptionRepository;
        private readonly IAnimalRepository _animalRepository;

        public AdoptionService(IAdoptionRepository adoptionRepository, IAnimalRepository animalRepository)
        {
            _adoptionRepository = adoptionRepository;
            _animalRepository = animalRepository;
        }

        public void CreateAdoption(int adopterId, int animalId, int statusId, string notes)
        {
            var adoption = new Adoption
            {
                AdopterId = adopterId,
                AnimalId = animalId,
                AdoptionDate = DateTime.Now,
                StatusId = statusId,
                Notes = notes
            };

            _adoptionRepository.Add(adoption);
            _adoptionRepository.SaveChanges();
            Console.WriteLine("Заявка на усыновление создана!");
        }

        public void UpdateAdoptionStatus(int adoptionId, int newStatusId)
        {
            var adoption = _adoptionRepository.GetById(adoptionId);
            if (adoption == null)
            {
                Console.WriteLine("Усыновление не найдено!");
                return;
            }

            adoption.StatusId = newStatusId;
            _adoptionRepository.Update(adoption);
            _adoptionRepository.SaveChanges();
            Console.WriteLine($"Статус усыновления обновлен!");
        }

        public void ReturnAnimal(int adoptionId, DateTime returnDate)
        {
            var adoption = _adoptionRepository.GetById(adoptionId);
            if (adoption == null)
            {
                Console.WriteLine("Усыновление не найдено!");
                return;
            }

            adoption.ReturnDate = returnDate;
            _adoptionRepository.Update(adoption);
            _adoptionRepository.SaveChanges();
            Console.WriteLine("Животное возвращено в приют!");
        }

        public Adoption GetAdoptionById(int adoptionId)
        {
            return _adoptionRepository.GetById(adoptionId);
        }

        public IEnumerable<Adoption> GetAllAdoptions()
        {
            return _adoptionRepository.GetAll();
        }

        public IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId)
        {
            return _adoptionRepository.GetAdoptionsByAdopter(adopterId);
        }

        public IEnumerable<Adoption> GetAdoptionsByStatus(int statusId)
        {
            return _adoptionRepository.GetAdoptionsByStatus(statusId);
        }
    }
}
