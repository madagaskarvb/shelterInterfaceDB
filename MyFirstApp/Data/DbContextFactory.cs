using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AnimalShelter.Data
{
    public class DbContextFactory
    {
        public static AnimalShelterContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnimalShelterContext>();
            optionsBuilder.UseNpgsql(connectionString);
            
            return new AnimalShelterContext(optionsBuilder.Options);
        }
    }
}
