using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
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
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        
        public virtual ICollection<AnimalVaccination> AnimalVaccinations { get; set; }
    }
}
