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
        [Column("full_name")]
        [MaxLength(200)]
        public string FullName { get; set; }
        
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
    }
}
