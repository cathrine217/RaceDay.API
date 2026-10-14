using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganiserId { get; set; }

        [ForeignKey("OrganiserId")]
        public User? Organiser { get; set; }

        [Required]
        public int EventTypeId { get; set; }

        [ForeignKey("EventTypeId")]
        public EventType? EventType { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }

        public int? MaxParticipants { get; set; }

        public string? Status { get; set; } // "Open", "Closed", "Cancelled"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}