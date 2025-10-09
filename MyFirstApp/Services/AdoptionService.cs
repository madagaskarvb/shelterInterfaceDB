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

        public void CreateAdoption(Adoption adoption)
        {
            try
            {
                // Проверка доступности животного
                var animal = _animalRepository.GetById(adoption.AnimalId);
                if (animal == null)
                {
                    Console.WriteLine("Животное не найдено!");
                    return;
                }

                if (animal.Status == "Adopted")
                {
                    Console.WriteLine("Это животное уже усыновлено!");
                    return;
                }

                _adoptionRepository.Add(adoption);
                _adoptionRepository.SaveChanges();
                Console.WriteLine("Заявка на усыновление создана!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании заявки: {ex.Message}");
            }
        }

        public Adoption GetAdoptionById(int id)
        {
            return _adoptionRepository.GetById(id);
        }

        public IEnumerable<Adoption> GetAllAdoptions()
        {
            return _adoptionRepository.GetAll();
        }

        public IEnumerable<Adoption> GetAdoptionsByStatus(string status)
        {
            return _adoptionRepository.GetAdoptionsByStatus(status);
        }

        public void ApproveAdoption(int adoptionId)
        {
            try
            {
                var adoption = _adoptionRepository.GetById(adoptionId);
                if (adoption == null)
                {
                    Console.WriteLine("Заявка не найдена!");
                    return;
                }

                adoption.Status = "Approved";
                _adoptionRepository.Update(adoption);

                // Обновление статуса животного
                var animal = _animalRepository.GetById(adoption.AnimalId);
                if (animal != null)
                {
                    animal.Status = "Adopted";
                    _animalRepository.Update(animal);
                }

                _adoptionRepository.SaveChanges();
                Console.WriteLine("Усыновление одобрено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public void RejectAdoption(int adoptionId, string reason)
        {
            try
            {
                var adoption = _adoptionRepository.GetById(adoptionId);
                if (adoption == null)
                {
                    Console.WriteLine("Заявка не найдена!");
                    return;
                }

                adoption.Status = "Rejected";
                adoption.Notes = reason;
                _adoptionRepository.Update(adoption);
                _adoptionRepository.SaveChanges();
                Console.WriteLine("Заявка отклонена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
