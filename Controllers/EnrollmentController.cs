using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public EnrolmentsController(RaceDayContext context) { _context = context; }

        [HttpPost("events/{eventId}/enrol")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<Enrolment>> Enrol(int eventId, [FromBody] EnrolRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var eventItem = await _context.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventItem == null) return NotFound("Event not found");

            var category = eventItem.Categories.FirstOrDefault(c => c.Id == request.CategoryId);
            if (category == null) return BadRequest("Category not found for this event");

            var existingEnrolment = await _context.Enrolments.FirstOrDefaultAsync(e => e.ParticipantId == userId && e.EventId == eventId);
            if (existingEnrolment != null) return BadRequest("You are already enrolled in this event");

            var enrolment = new Enrolment
            {
                ParticipantId = userId,
                EventId = eventId,
                CategoryId = request.CategoryId,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }

        [HttpGet("users/me/enrolments")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetMyEnrolments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            return await _context.Enrolments.Include(e => e.Event).Include(e => e.Category).Where(e => e.ParticipantId == userId).ToListAsync();
        }

        [HttpGet("events/{eventId}/enrolments")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEventEnrolments(int eventId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null) return NotFound("Event not found");
            if (eventItem.OrganiserId != userId) return StatusCode(403, "You are not the organiser");

            return await _context.Enrolments.Include(e => e.Participant).Include(e => e.Category).Where(e => e.EventId == eventId).ToListAsync();
        }
    }

    public class EnrolRequest { public int CategoryId { get; set; } }
}