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
        public DbSet<AnimalStatus> AnimalStatuses { get; set; }
        public DbSet<AdoptionStatus> AdoptionStatuses { get; set; }

        public AnimalShelterContext(DbContextOptions<AnimalShelterContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи 1:1 между Animal и MedicalRecord
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Animal)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(m => m.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи 1:M между AnimalStatus и Animals
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.AnimalStatus)
                .WithMany(s => s.Animals)
                .HasForeignKey(a => a.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

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

            // Настройка связи 1:M между AdoptionStatus и Adoptions
            modelBuilder.Entity<Adoption>()
                .HasOne(ad => ad.AdoptionStatus)
                .WithMany(s => s.Adoptions)
                .HasForeignKey(ad => ad.StatusId)
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

            modelBuilder.Entity<AnimalStatus>()
                .HasIndex(s => s.StatusName)
                .IsUnique();

            modelBuilder.Entity<AdoptionStatus>()
                .HasIndex(s => s.StatusName)
                .IsUnique();

            modelBuilder.Entity<MedicalRecord>()
                .HasIndex(m => m.RecordNumber)
                .IsUnique();

            modelBuilder.Entity<Vaccination>()
                .HasIndex(v => v.VaccineName)
                .IsUnique();

            // Индексы для оптимизации поиска
            modelBuilder.Entity<Animal>()
                .HasIndex(a => a.StatusId);

            modelBuilder.Entity<Animal>()
                .HasIndex(a => a.Species);

            modelBuilder.Entity<Adoption>()
                .HasIndex(ad => ad.AdopterId);

            modelBuilder.Entity<Adoption>()
                .HasIndex(ad => ad.AnimalId);

            modelBuilder.Entity<Adoption>()
                .HasIndex(ad => ad.StatusId);

            modelBuilder.Entity<AnimalVaccination>()
                .HasIndex(av => av.AnimalId);

            modelBuilder.Entity<AnimalVaccination>()
                .HasIndex(av => av.VaccinationId);

            // Составной уникальный индекс для AnimalVaccination
            modelBuilder.Entity<AnimalVaccination>()
                .HasIndex(av => new { av.AnimalId, av.VaccinationId, av.VaccinationDate })
                .IsUnique();
        }
    }
}
