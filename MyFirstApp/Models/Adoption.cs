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
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }

        [Column("notes")]
        public string Notes { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("AdopterId")]
        public virtual Adopter Adopter { get; set; }

        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        [ForeignKey("StatusId")]
        public virtual AdoptionStatus AdoptionStatus { get; set; }
    }
}
