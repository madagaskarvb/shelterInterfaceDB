using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
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
        public string Address { get; set; }

        [Column("registration_date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Adoption> Adoptions { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
