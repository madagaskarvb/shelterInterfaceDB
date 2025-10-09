using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
    [Table("adoptions")]
    public class Adoption
    {
        [Key]
        [Column("adoption_id")]
        public int AdoptionId { get; set; }
        
        [Required]
        [Column("adopter_id")]
        public int AdopterId { get; set; }
        
        [Required]
        [Column("animal_id")]
        public int AnimalId { get; set; }
        
        [Column("adoption_date")]
        public DateTime AdoptionDate { get; set; } = DateTime.Now;
        
        [Required]
        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        
        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }
        
        [Column("notes")]
        public string Notes { get; set; }
        
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        
        public virtual Adopter Adopter { get; set; }
        public virtual Animal Animal { get; set; }
    }
}
