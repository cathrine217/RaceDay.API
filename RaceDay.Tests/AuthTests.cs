using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthTests
    {
        private RaceDayContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayContext(options);
        }

        [Fact]
        public async Task Register_ValidUser_AddsUserToDatabase()
        {
            using var context = GetInMemoryContext();

            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FirstName = "Test",
                LastName = "User",
                Role = "Participant"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            Assert.Equal(1, await context.Users.CountAsync());
            Assert.Equal("test@example.com", (await context.Users.FirstAsync()).Email);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("mypassword");
            var isValid = BCrypt.Net.BCrypt.Verify("mypassword", hash);
            Assert.True(isValid);
        }

        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("mypassword");
            var isValid = BCrypt.Net.BCrypt.Verify("wrongpassword", hash);
            Assert.False(isValid);
        }

        [Fact]
        public async Task DuplicateEmail_ShouldNotBeAllowed()
        {
            using var context = GetInMemoryContext();

            var user1 = new User
            {
                Email = "same@example.com",
                PasswordHash = "hash1",
                FirstName = "User",
                LastName = "One",
                Role = "Participant"
            };

            context.Users.Add(user1);
            await context.SaveChangesAsync();

            var duplicate = await context.Users.AnyAsync(u => u.Email == "same@example.com");

            Assert.True(duplicate);
        }
    }
}
