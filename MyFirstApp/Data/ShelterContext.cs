using Microsoft.EntityFrameworkCore;
using AnimalShelterCLI.Models;

namespace AnimalShelterCLI.Data
{
    public class ShelterContext : DbContext
    {
        public DbSet<AnimalStatus> AnimalStatuses { get; set; }
        public DbSet<AdoptionStatus> AdoptionStatuses { get; set; }
        public DbSet<Adopter> Adopters { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<AnimalVaccination> AnimalVaccinations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=138.124.14.1;Port=5432;Database=Animal House;Username=myuser;Password=mypassword;Include Error Detail=true"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка каскадного удаления и ограничений
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Status)
                .WithMany(s => s.Animals)
                .HasForeignKey(a => a.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Animal)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(mr => mr.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Adopter)
                .WithMany(ad => ad.Adoptions)
                .HasForeignKey(a => a.AdopterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Animal)
                .WithMany(an => an.Adoptions)
                .HasForeignKey(a => a.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AnimalVaccination>()
                .HasOne(av => av.MedicalRecord)
                .WithMany(mr => mr.AnimalVaccinations)
                .HasForeignKey(av => av.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
