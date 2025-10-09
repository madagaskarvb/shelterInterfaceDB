using System.Linq;
using AnimalShelter.Data;
using AnimalShelter.Interfaces;
using AnimalShelter.Models;

namespace AnimalShelter.Repositories
{
    public class AdopterRepository : Repository<Adopter>, IAdopterRepository
    {
        public AdopterRepository(AnimalShelterContext context) : base(context)
        {
        }

        public Adopter GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(a => a.Email == email);
        }
    }
}
