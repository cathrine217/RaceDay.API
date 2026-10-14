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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public ResultsController(RaceDayContext context) { _context = context; }

        [HttpPost("enrolments/{enrolmentId}/result")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Result>> AddResult(int enrolmentId, Result result)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var enrolment = await _context.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.Id == enrolmentId);

            if (enrolment == null) return NotFound("Enrolment not found");
            if (enrolment.Event?.OrganiserId != userId) return StatusCode(403, "You are not the organiser");

            var existingResult = await _context.Results.FirstOrDefaultAsync(r => r.EnrolmentId == enrolmentId);
            if (existingResult != null)
            {
                existingResult.FinishTime = result.FinishTime;
                existingResult.Position = result.Position;
                existingResult.CategoryPosition = result.CategoryPosition;
                existingResult.Status = result.Status;
                await _context.SaveChangesAsync();
                return Ok(existingResult);
            }

            result.EnrolmentId = enrolmentId;
            result.CreatedAt = DateTime.UtcNow;
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        [HttpGet("events/{eventId}/results")]
        public async Task<ActionResult<IEnumerable<object>>> GetEventResults(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists) return NotFound("Event not found");

            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Participant)
                .Include(r => r.Enrolment).ThenInclude(e => e.Category)
                .Where(r => r.Enrolment.EventId == eventId)
                .Select(r => new {
                    r.Id,
                    ParticipantName = r.Enrolment.Participant.FirstName + " " + r.Enrolment.Participant.LastName,
                    Category = r.Enrolment.Category.Name,
                    r.FinishTime,
                    r.Position,
                    r.CategoryPosition,
                    r.Status
                }).ToListAsync();

            return Ok(results);
        }

        [HttpGet("users/me/results")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyResults()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e.Category)
                .Where(r => r.Enrolment.ParticipantId == userId)
                .Select(r => new {
                    r.Id,
                    EventName = r.Enrolment.Event.Name,
                    EventDate = r.Enrolment.Event.Date,
                    Category = r.Enrolment.Category.Name,
                    r.FinishTime,
                    r.Position,
                    r.CategoryPosition,
                    r.Status
                }).ToListAsync();

            return Ok(results);
        }
    }
}