using Microsoft.EntityFrameworkCore;
using AnimalShelter.Models;

namespace AnimalShelter.Data
{
    public class AnimalShelterContext : DbContext
    {
        public DbSet<Adopter> Adopters { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<AnimalVaccination> AnimalVaccinations { get; set; }

        public AnimalShelterContext(DbContextOptions<AnimalShelterContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи 1:1 между Animal и MedicalRecord
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.MedicalRecord)
                .WithOne(m => m.Animal)
                .HasForeignKey<MedicalRecord>(m => m.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи 1:M между Adopter и Adoptions
            modelBuilder.Entity<Adoption>()
                .HasOne(ad => ad.Adopter)
                .WithMany(a => a.Adoptions)
                .HasForeignKey(ad => ad.AdopterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи 1:M между Animal и Adoptions
            modelBuilder.Entity<Adoption>()
                .HasOne(ad => ad.Animal)
                .WithMany(a => a.Adoptions)
                .HasForeignKey(ad => ad.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи M:N через AnimalVaccinations
            modelBuilder.Entity<AnimalVaccination>()
                .HasOne(av => av.MedicalRecord)
                .WithMany(m => m.AnimalVaccinations)
                .HasForeignKey(av => av.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnimalVaccination>()
                .HasOne(av => av.Vaccination)
                .WithMany(v => v.AnimalVaccinations)
                .HasForeignKey(av => av.VaccinationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Уникальные индексы
            modelBuilder.Entity<Adopter>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<MedicalRecord>()
                .HasIndex(m => m.RecordNumber)
                .IsUnique();

            modelBuilder.Entity<Vaccination>()
                .HasIndex(v => v.VaccineName)
                .IsUnique();
        }
    }
}
