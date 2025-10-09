using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Models
{
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

        public virtual ICollection<Animal> Animals { get; set; }
    }
}
