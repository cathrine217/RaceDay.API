using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        [ForeignKey("ParticipantId")]
        public User? Participant { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Confirmed"; // "Confirmed", "Pending", "Cancelled"
    }
}