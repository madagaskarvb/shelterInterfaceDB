using System.Collections.Generic;
using AnimalShelter.Models;

namespace AnimalShelter.Interfaces
{
    public interface IAnimalRepository : IRepository<Animal>
    {
        IEnumerable<Animal> GetAnimalsByStatus(int statusId);
    }
}
