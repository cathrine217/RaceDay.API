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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;
        public CategoriesController(RaceDayContext context) { _context = context; }

        [HttpGet("events/{eventId}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists) return NotFound("Event not found");

            return await _context.Categories.Where(c => c.EventId == eventId).ToListAsync();
        }

        [HttpPost("events/{eventId}/categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> CreateCategory(int eventId, Category category)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null) return NotFound("Event not found");
            if (eventItem.OrganiserId != userId) return StatusCode(403, "You are not the organiser of this event");

            category.EventId = eventId;
            category.CreatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { eventId }, category);
        }

        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id) return BadRequest();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var existingCategory = await _context.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null) return NotFound();
            if (existingCategory.Event?.OrganiserId != userId) return StatusCode(403, "You are not the organiser");

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Fee = category.Fee;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var category = await _context.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();
            if (category.Event?.OrganiserId != userId) return StatusCode(403, "You are not the organiser");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}