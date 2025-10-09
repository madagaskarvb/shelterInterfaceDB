using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
    [Table("medical_records")]
    public class MedicalRecord
    {
        [Key]
        [Column("animal_id")]
        [ForeignKey("Animal")]
        public int AnimalId { get; set; }
        
        [Required]
        [Column("record_number")]
        [MaxLength(50)]
        public string RecordNumber { get; set; }
        
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
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
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        
        public virtual Animal Animal { get; set; }
        public virtual ICollection<AnimalVaccination> AnimalVaccinations { get; set; }
    }
}
