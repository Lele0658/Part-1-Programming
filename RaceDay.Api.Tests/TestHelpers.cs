using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Tests;

public static class TestHelpers
{
    public static RaceDayDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new RaceDayDbContext(options);
    }

    public static void AttachUser(ControllerBase controller, int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Email, $"{role.ToLower()}@test.com"),
            new Claim(ClaimTypes.Name, $"Test {role}")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    public static User CreateUser(int id, string role, string email = "")
    {
        return new User
        {
            Id = id,
            Email = string.IsNullOrEmpty(email) ? $"{role.ToLower()}{id}@test.com" : email,
            FullName = $"Test {role} {id}",
            Role = role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        };
    }
}