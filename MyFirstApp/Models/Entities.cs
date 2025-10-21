using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelterCLI.Models
{

    public enum AdopterApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }



    [Table("animal_statuses")]
    public class AnimalStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }
        [Required]
        [Column("status_name")]
        [MaxLength(50)]
        public string StatusName { get; set; }
        [Column("description")]
        public string Description { get; set; }
        public ICollection<Animal> Animals { get; set; }
    }

    [Table("adoption_statuses")]
    public class AdoptionStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }
        [Required]
        [Column("status_name")]
        [MaxLength(50)]
        public string StatusName { get; set; }
        [Column("description")]
        public string Description { get; set; }
        public ICollection<Adoption> Adoptions { get; set; }
    }

    [Table("adopters")]
    public class Adopter
    {
        [Key]
        [Column("adopter_id")]
        public int AdopterId { get; set; }
        [Required]
        [Column("first_name")]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [Column("last_name")]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; }
        [Column("phone")]
        [MaxLength(20)]
        public string Phone { get; set; }
        [Column("address")]
        [MaxLength(200)]
        public string Address { get; set; }
        [Column("registration_date")]
        public DateTime? RegistrationDate { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("approval_status")]
        public AdopterApprovalStatus ApprovalStatus { get; set; } = AdopterApprovalStatus.Pending;

        public ICollection<Adoption> Adoptions { get; set; }
    }

    [Table("animals")]
    public class Animal
    {
        [Key]
        [Column("animal_id")]
        public int AnimalId { get; set; }
        
        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Column("age")]
        public int? Age { get; set; }
        
        [Required]
        [Column("species")]
        [MaxLength(50)]
        public string Species { get; set; }
        
        [Column("breed")]
        [MaxLength(100)]
        public string Breed { get; set; }
        
        [Column("gender")]
        [MaxLength(10)]
        public string Gender { get; set; }
        
        [Column("date_admitted")]
        public DateTime DateAdmitted { get; set; }
        
        [Column("status_id")]
        public int StatusId { get; set; }
        
        [Column("description")]
        public string Description { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey("StatusId")]
        public AnimalStatus Status { get; set; }
        
        public MedicalRecord MedicalRecord { get; set; }
        public ICollection<Adoption> Adoptions { get; set; }
    }

    [Table("medical_records")]
    public class MedicalRecord
    {
        [Key]
        [Column("animal_id")]
        public int AnimalId { get; set; }
        
        [Column("owner_id")]
        public int? OwnerId { get; set; }
        
        [Required]
        [Column("record_number")]
        [MaxLength(50)]
        public string RecordNumber { get; set; }
        
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        
        [Column("blood_type")]
        [MaxLength(10)]
        public string BloodType { get; set; }
        
        [Column("allergies")]
        public string Allergies { get; set; }
        
        [Column("chronic_conditions")]
        public string ChronicConditions { get; set; }
        
        [Column("special_needs")]
        public string SpecialNeeds { get; set; }
        
        [Column("veterinarian_notes")]
        public string VeterinarianNotes { get; set; }
        
        [Column("last_checkup_date")]
        public DateTime? LastCheckupDate { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [ForeignKey("AnimalId")]
        public Animal Animal { get; set; }
        
        [ForeignKey("OwnerId")]
        public Adopter Owner { get; set; }
        
        public ICollection<AnimalVaccination> AnimalVaccinations { get; set; }
    }

    [Table("adoptions")]
    public class Adoption
    {
        [Key]
        [Column("adoption_id")]
        public int AdoptionId { get; set; }
        
        [Column("adopter_id")]
        public int AdopterId { get; set; }
        
        [Column("animal_id")]
        public int AnimalId { get; set; }
        
        [Column("adoption_date")]
        public DateTime AdoptionDate { get; set; }
        
        [Column("status_id")]
        public int StatusId { get; set; }
        
        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }
        
        [Column("notes")]
        public string Notes { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey("AdopterId")]
        public Adopter Adopter { get; set; }
        
        [ForeignKey("AnimalId")]
        public Animal Animal { get; set; }
        
        [ForeignKey("StatusId")]
        public AdoptionStatus Status { get; set; }
    }

    [Table("vaccinations")]
    public class Vaccination
    {
        [Key]
        [Column("vaccination_id")]
        public int VaccinationId { get; set; }
        
        [Required]
        [Column("vaccine_name")]
        [MaxLength(100)]
        public string VaccineName { get; set; }
        
        [Column("description")]
        public string Description { get; set; }
        
        [Column("manufacturer")]
        [MaxLength(100)]
        public string Manufacturer { get; set; }
        
        [Column("validity_period_months")]
        public int? ValidityPeriodMonths { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        public ICollection<AnimalVaccination> AnimalVaccinations { get; set; }
    }

    [Table("animal_vaccinations")]
    public class AnimalVaccination
    {
        [Key]
        [Column("vaccination_record_id")]
        public int VaccinationRecordId { get; set; }
        
        [Column("animal_id")]
        public int AnimalId { get; set; }
        
        [Column("vaccination_id")]
        public int VaccinationId { get; set; }
        
        [Column("vaccination_date")]
        public DateTime VaccinationDate { get; set; }
        
        [Column("next_due_date")]
        public DateTime? NextDueDate { get; set; }
        
        [Column("batch_number")]
        [MaxLength(50)]
        public string BatchNumber { get; set; }
        
        [Column("veterinarian_name")]
        [MaxLength(100)]
        public string VeterinarianName { get; set; }
        
        [Column("notes")]
        public string Notes { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey("AnimalId")]
        public MedicalRecord MedicalRecord { get; set; }
        
        [ForeignKey("VaccinationId")]
        public Vaccination Vaccination { get; set; }
    }
}
