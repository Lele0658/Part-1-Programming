using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Tests;

public class EnrolmentsControllerTests
{
    private static async Task<(Data.RaceDayDbContext ctx, Event ev, Category cat)>
        SeedEventAndCategory(Data.RaceDayDbContext ctx)
    {
        ctx.Users.Add(TestHelpers.CreateUser(1, "Organiser"));
        ctx.Users.Add(TestHelpers.CreateUser(2, "Participant"));
        var ev = new Event
        {
            Id = 1,
            OrganiserId = 1,
            Name = "Test Event",
            Date = DateTime.UtcNow.AddDays(10)
        };
        var cat = new Category
        {
            Id = 1,
            EventId = 1,
            Name = "10km",
            AgeGroup = "Open",
            Distance = 10
        };
        ctx.Events.Add(ev);
        ctx.Categories.Add(cat);
        await ctx.SaveChangesAsync();
        return (ctx, ev, cat);
    }

    [Fact]
    public async Task Enrol_AsParticipant_Returns201()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEventAndCategory(ctx);

        var controller = new EnrolmentsController(ctx);
        TestHelpers.AttachUser(controller, 2, "Participant");

        var result = await controller.Enrol(new EnrolmentDto { EventId = 1, CategoryId = 1 });

        var created = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, created.StatusCode);

        var response = Assert.IsType<EnrolmentResponseDto>(created.Value);
        Assert.Equal(2, response.ParticipantId);
        Assert.Equal("10km", response.CategoryName);
    }

    [Fact]
public void Enrol_RequiresParticipantRole()
{
    var method = typeof(EnrolmentsController).GetMethod(nameof(EnrolmentsController.Enrol))!;
    var attr = method.GetCustomAttribute<AuthorizeAttribute>();
    Assert.NotNull(attr);
    Assert.Equal("Participant", attr!.Roles);
}

    [Fact]
    public async Task Enrol_Duplicate_Returns409()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEventAndCategory(ctx);
        ctx.Enrolments.Add(new Enrolment
        {
            ParticipantId = 2,
            EventId = 1,
            CategoryId = 1
        });
        await ctx.SaveChangesAsync();

        var controller = new EnrolmentsController(ctx);
        TestHelpers.AttachUser(controller, 2, "Participant");

        var result = await controller.Enrol(new EnrolmentDto { EventId = 1, CategoryId = 1 });
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task MyEnrolments_ReturnsOnlyCurrentUsersEnrolments()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEventAndCategory(ctx);
        ctx.Users.Add(TestHelpers.CreateUser(3, "Participant"));
        ctx.Enrolments.Add(new Enrolment { Id = 1, ParticipantId = 2, EventId = 1, CategoryId = 1 });
        ctx.Enrolments.Add(new Enrolment { Id = 2, ParticipantId = 3, EventId = 1, CategoryId = 1 });
        await ctx.SaveChangesAsync();

        var controller = new EnrolmentsController(ctx);
        TestHelpers.AttachUser(controller, 2, "Participant");

        var result = await controller.MyEnrolments();
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = (IEnumerable<EnrolmentResponseDto>)ok.Value!;
        Assert.Single(list);
        Assert.Equal(2, list.First().ParticipantId);
    }
}
