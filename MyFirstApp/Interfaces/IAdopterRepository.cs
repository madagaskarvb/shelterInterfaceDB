using AnimalShelter.Models;

namespace AnimalShelter.Interfaces
{
    public interface IAdopterRepository : IRepository<Adopter>
    {
        Adopter GetByEmail(string email);
    }
}
