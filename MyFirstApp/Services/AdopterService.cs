using System;
using System.Collections.Generic;
using AnimalShelter.Interfaces;
using AnimalShelter.Models;

namespace AnimalShelter.Services
{
    public class AdopterService
    {
        private readonly IAdopterRepository _adopterRepository;

        public AdopterService(IAdopterRepository adopterRepository)
        {
            _adopterRepository = adopterRepository;
        }

        public void AddAdopter(Adopter adopter)
        {
            try
            {
                // Проверка на дубликат email
                var existingAdopter = _adopterRepository.GetByEmail(adopter.Email);
                if (existingAdopter != null)
                {
                    Console.WriteLine("Усыновитель с таким email уже существует!");
                    return;
                }

                _adopterRepository.Add(adopter);
                _adopterRepository.SaveChanges();
                Console.WriteLine($"Усыновитель '{adopter.FullName}' успешно зарегистрирован!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении усыновителя: {ex.Message}");
            }
        }

        public Adopter GetAdopterById(int id)
        {
            return _adopterRepository.GetById(id);
        }

        public IEnumerable<Adopter> GetAllAdopters()
        {
            return _adopterRepository.GetAll();
        }

        public Adopter GetAdopterByEmail(string email)
        {
            return _adopterRepository.GetByEmail(email);
        }

        public void UpdateAdopter(Adopter adopter)
        {
            try
            {
                _adopterRepository.Update(adopter);
                _adopterRepository.SaveChanges();
                Console.WriteLine("Информация об усыновителе обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении: {ex.Message}");
            }
        }

        public void DeleteAdopter(int id)
        {
            try
            {
                _adopterRepository.Delete(id);
                _adopterRepository.SaveChanges();
                Console.WriteLine("Усыновитель удален из системы!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}
