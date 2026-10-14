using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class EventType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // "Running", "Cycling", "Walking"
    }
}