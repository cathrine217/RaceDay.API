using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly IConfiguration _configuration; // Added for Azure

        public EventsController(RaceDayContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.Include(e => e.EventType).Include(e => e.Categories).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null) return NotFound();
            return eventItem;
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Event>> CreateEvent(Event eventItem)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            eventItem.OrganiserId = userId;
            eventItem.CreatedAt = DateTime.UtcNow;

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, eventItem);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(int id, Event eventItem)
        {
            if (id != eventItem.Id) return BadRequest();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound();
            if (existingEvent.OrganiserId != userId) return StatusCode(403, "You are not the organiser of this event");

            existingEvent.Name = eventItem.Name;
            existingEvent.Description = eventItem.Description;
            existingEvent.Date = eventItem.Date;
            existingEvent.Location = eventItem.Location;
            existingEvent.MaxParticipants = eventItem.MaxParticipants;
            existingEvent.Status = eventItem.Status;
            existingEvent.EventTypeId = eventItem.EventTypeId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null) return NotFound();
            if (eventItem.OrganiserId != userId) return StatusCode(403, "You are not the organiser of this event");

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // THIS IS NOW CORRECTLY PLACED AT THE CLASS LEVEL
        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null || file == null) return BadRequest("Event not found or no file provided");

            var connectionString = _configuration["AzureBlobStorage"];
            if (string.IsNullOrEmpty(connectionString)) return StatusCode(500, "Azure connection string missing");

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("eventimages");
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            ev.ImageUrl = blobClient.Uri.ToString();
            await _context.SaveChangesAsync();

            return Ok(new { url = ev.ImageUrl });
        }
    }
}