using System.Collections.Generic;
using AnimalShelter.Models;

namespace AnimalShelter.Interfaces
{
    public interface IAdoptionRepository : IRepository<Adoption>
    {
        IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId);
        IEnumerable<Adoption> GetAdoptionsByStatus(int statusId);
    }
}
