using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventTests
    {
        private RaceDayContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public async Task CreateEvent_ValidData_AddsEventToDatabase()
        {
            using var context = GetInMemoryContext();

            var organiser = new User { Email = "org@test.com", PasswordHash = "hash", FirstName = "Org", LastName = "User", Role = "Organiser" };
            var eventType = new EventType { Name = "Running" };

            context.Users.Add(organiser);
            context.EventTypes.Add(eventType);
            await context.SaveChangesAsync();

            var newEvent = new Event
            {
                OrganiserId = organiser.Id,
                EventTypeId = eventType.Id,
                Name = "Test Marathon",
                Date = System.DateTime.UtcNow.AddDays(30),
                Status = "Open"
            };

            context.Events.Add(newEvent);
            await context.SaveChangesAsync();

            Assert.Equal(1, await context.Events.CountAsync());
        }
    }
}
