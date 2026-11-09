using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Tests;

public class EventsControllerTests
{
    [Fact]
    public async Task Create_AsOrganiser_Returns201()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        ctx.Users.Add(TestHelpers.CreateUser(1, "Organiser"));
        await ctx.SaveChangesAsync();

        var controller = new EventsController(ctx);
        TestHelpers.AttachUser(controller, 1, "Organiser");

        var result = await controller.Create(new EventDto
        {
            Name = "Comrades",
            Description = "Ultra marathon",
            Date = DateTime.UtcNow.AddDays(30),
            Location = "Durban",
            Distance = 89.0,
            EventType = "run"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<EventResponseDto>(created.Value);
        Assert.Equal("Comrades", dto.Name);
    }

    [Fact]
public void Create_RequiresOrganiserRole()
{
    var method = typeof(EventsController).GetMethod(nameof(EventsController.Create))!;
    var attr = method.GetCustomAttribute<AuthorizeAttribute>();
    Assert.NotNull(attr);
    Assert.Equal("Organiser", attr!.Roles);
}

    [Fact]
    public async Task Create_WithoutName_Returns400()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        ctx.Users.Add(TestHelpers.CreateUser(1, "Organiser"));
        await ctx.SaveChangesAsync();

        var controller = new EventsController(ctx);
        TestHelpers.AttachUser(controller, 1, "Organiser");

        var result = await controller.Create(new EventDto { Name = "" });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WithUnknownId_Returns404()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        var controller = new EventsController(ctx);

        var result = await controller.GetById(999);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ByNonOwner_Returns403()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        ctx.Users.AddRange(
            TestHelpers.CreateUser(1, "Organiser"),
            TestHelpers.CreateUser(2, "Organiser")
        );
        ctx.Events.Add(new Event
        {
            Id = 10,
            OrganiserId = 1,
            Name = "Owned by 1",
            Date = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        var controller = new EventsController(ctx);
        TestHelpers.AttachUser(controller, 2, "Organiser");

        var result = await controller.Update(10, new EventDto { Name = "Hacked" });
        Assert.IsType<ForbidResult>(result);
    }
}
