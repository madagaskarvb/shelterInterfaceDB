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

        public void RegisterAdopter(string firstName, string lastName, string email, string phone, string address)
        {
            var adopter = new Adopter
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Address = address,
                RegistrationDate = DateTime.Now
            };

            _adopterRepository.Add(adopter);
            _adopterRepository.SaveChanges();
            Console.WriteLine($"Усыновитель {firstName} {lastName} зарегистрирован успешно!");
        }

        public void UpdateAdopter(int adopterId, string firstName, string lastName, string email, string phone, string address)
        {
            var adopter = _adopterRepository.GetById(adopterId);
            if (adopter == null)
            {
                Console.WriteLine("Усыновитель не найден!");
                return;
            }

            if (!string.IsNullOrEmpty(firstName)) adopter.FirstName = firstName;
            if (!string.IsNullOrEmpty(lastName)) adopter.LastName = lastName;
            if (!string.IsNullOrEmpty(email)) adopter.Email = email;
            if (!string.IsNullOrEmpty(phone)) adopter.Phone = phone;
            if (!string.IsNullOrEmpty(address)) adopter.Address = address;

            _adopterRepository.Update(adopter);
            _adopterRepository.SaveChanges();
            Console.WriteLine($"Данные усыновителя {adopter.FullName} обновлены!");
        }

        public void DeleteAdopter(int adopterId)
        {
            _adopterRepository.Delete(adopterId);
            _adopterRepository.SaveChanges();
            Console.WriteLine("Усыновитель удален!");
        }

        public Adopter GetAdopterById(int adopterId)
        {
            return _adopterRepository.GetById(adopterId);
        }

        public IEnumerable<Adopter> GetAllAdopters()
        {
            return _adopterRepository.GetAll();
        }
    }
}
