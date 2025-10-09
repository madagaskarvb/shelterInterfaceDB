using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
    [Table("animal_vaccinations")]
    public class AnimalVaccination
    {
        [Key]
        [Column("vaccination_record_id")]
        public int VaccinationRecordId { get; set; }
        
        [Required]
        [Column("animal_id")]
        public int AnimalId { get; set; }
        
        [Required]
        [Column("vaccination_id")]
        public int VaccinationId { get; set; }
        
        [Column("vaccination_date")]
        public DateTime VaccinationDate { get; set; } = DateTime.Now;
        
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
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        
        public virtual MedicalRecord MedicalRecord { get; set; }
        public virtual Vaccination Vaccination { get; set; }
    }
}
