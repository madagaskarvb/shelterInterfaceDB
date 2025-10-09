using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
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
        public DateTime DateAdmitted { get; set; } = DateTime.Now;

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("StatusId")]
        public virtual AnimalStatus AnimalStatus { get; set; }

        public virtual MedicalRecord MedicalRecord { get; set; }
        public virtual ICollection<Adoption> Adoptions { get; set; }
    }
}
