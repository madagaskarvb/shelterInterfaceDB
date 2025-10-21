using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                .Where(a => a.FirstName.Contains(searchTerm)
                         || a.LastName.Contains(searchTerm)
                         || a.Email.Contains(searchTerm))
                .ToList();
        }

        public void AddAdopter(Adopter adopter)
        {
            if (!ValidateAdopter(adopter, out string error))
                throw new ArgumentException(error);

            adopter.RegistrationDate = DateTime.UtcNow;
            adopter.CreatedAt = DateTime.UtcNow;
            _context.Adopters.Add(adopter);
            _context.SaveChanges();
        }

        public void UpdateAdopter(Adopter adopter)
        {
            if (!ValidateAdopter(adopter, out string error))
                throw new ArgumentException(error);

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

        public void ApproveAdopter(int id)
        {
            var adopter = GetAdopterById(id);
            if (adopter == null) throw new Exception("Усыновитель не найден.");
            adopter.ApprovalStatus = AdopterApprovalStatus.Approved;
            _context.SaveChanges();
        }

        public void RejectAdopter(int id)
        {
            var adopter = GetAdopterById(id);
            if (adopter == null) throw new Exception("Усыновитель не найден.");
            adopter.ApprovalStatus = AdopterApprovalStatus.Rejected;
            _context.SaveChanges();
        }

        public List<Adopter> GetPendingAdopters()
        {
            return _context.Adopters
                .Where(a => a.ApprovalStatus == AdopterApprovalStatus.Pending)
                .Include(a => a.Adoptions)
                .ToList();
        }

        public static bool ValidateAdopter(Adopter adopter, out string error)
        {
            error = null;

            if (!ValidateEmail(adopter.Email))
            {
                error = "Некорректный e-mail. Пример: example@gmail.com";
                return false;
            }

            if (!ValidatePhone(adopter.Phone))
            {
                error = "Некорректный телефон. Примеры (RU): +79161234567, 84951234567, +44...";
                return false;
            }

            if (string.IsNullOrWhiteSpace(adopter.Address))
            {
                error = "Адрес обязателен.";
                return false;
            }

            return true;
        }

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public static bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            const string pattern = @"^(\+?[1-9]\d{5,14}|8\d{10})$";
            var compact = Regex.Replace(phone, @"\s+", "");
            return Regex.IsMatch(compact, pattern);
        }
    }
}
