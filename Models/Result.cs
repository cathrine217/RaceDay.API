using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        [ForeignKey("EnrolmentId")]
        public Enrolment? Enrolment { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public int? CategoryPosition { get; set; }

        public string? Status { get; set; } // "Completed", "DNF", "DNS"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
