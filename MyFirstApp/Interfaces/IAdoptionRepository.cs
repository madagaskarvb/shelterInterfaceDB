using System;
using System.Collections.Generic;
using AnimalShelter.Models;

namespace AnimalShelter.Interfaces
{
    public interface IAdoptionRepository : IRepository<Adoption>
    {
        IEnumerable<Adoption> GetAdoptionsByStatus(string status);
        IEnumerable<Adoption> GetAdoptionsByDateRange(DateTime startDate, DateTime endDate);
        IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId);
    }
}
