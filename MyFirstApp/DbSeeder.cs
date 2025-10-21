// DbSeeder.cs
using System.Linq;
using AnimalShelterCLI.Data;
using AnimalShelterCLI.Models;

public static class DbSeeder
{
    public static void EnsureSeeded(ShelterContext ctx)
    {
        ctx.Database.EnsureCreated();

        // Животные: гарантируем наличие StatusId = 1
        if (!ctx.AnimalStatuses.Any(s => s.StatusId == 1))
        {
            ctx.AnimalStatuses.Add(new AnimalStatus
            {
                StatusId = 1,
                StatusName = "Available",
                Description = "Доступно"
            });
        }
        // Можно добавить и другие
        if (!ctx.AnimalStatuses.Any(s => s.StatusId == 2))
            ctx.AnimalStatuses.Add(new AnimalStatus { StatusId = 2, StatusName = "Adopted" });
        if (!ctx.AnimalStatuses.Any(s => s.StatusId == 3))
            ctx.AnimalStatuses.Add(new AnimalStatus { StatusId = 3, StatusName = "On Hold" });

        // Усыновления: гарантируем наличие StatusId = 1
        if (!ctx.AdoptionStatuses.Any(s => s.StatusId == 1))
        {
            ctx.AdoptionStatuses.Add(new AdoptionStatus
            {
                StatusId = 1,
                StatusName = "Pending",
                Description = "В ожидании"
            });
        }
        if (!ctx.AdoptionStatuses.Any(s => s.StatusId == 2))
            ctx.AdoptionStatuses.Add(new AdoptionStatus { StatusId = 2, StatusName = "Approved" });
        if (!ctx.AdoptionStatuses.Any(s => s.StatusId == 3))
            ctx.AdoptionStatuses.Add(new AdoptionStatus { StatusId = 3, StatusName = "Rejected" });

        ctx.SaveChanges();
    }
}
