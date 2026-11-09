using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Tests;

public class ResultsControllerTests
{
    private static async Task<(Data.RaceDayDbContext ctx, Enrolment enrolment)>
        SeedEnrolment(Data.RaceDayDbContext ctx)
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
        var cat = new Category { Id = 1, EventId = 1, Name = "10km", Distance = 10 };
        var enrol = new Enrolment
        {
            Id = 1,
            ParticipantId = 2,
            EventId = 1,
            CategoryId = 1
        };
        ctx.Events.Add(ev);
        ctx.Categories.Add(cat);
        ctx.Enrolments.Add(enrol);
        await ctx.SaveChangesAsync();
        return (ctx, enrol);
    }

    [Fact]
    public async Task Capture_AsOrganiser_Returns201()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEnrolment(ctx);

        var controller = new ResultsController(ctx);
        TestHelpers.AttachUser(controller, 1, "Organiser");

        var result = await controller.Capture(new ResultDto
        {
            EnrolmentId = 1,
            FinishTime = TimeSpan.FromMinutes(42),
            Position = 5
        });

        var created = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, created.StatusCode);
        var dto = Assert.IsType<ResultResponseDto>(created.Value);
        Assert.Equal(5, dto.Position);
    }

    [Fact]
    public async Task Capture_AsParticipant_Returns403()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEnrolment(ctx);

        var controller = new ResultsController(ctx);
        TestHelpers.AttachUser(controller, 2, "Participant");

        var result = await controller.Capture(new ResultDto
        {
            EnrolmentId = 1,
            FinishTime = TimeSpan.FromMinutes(42),
            Position = 5
        });

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Capture_ByNonOwnerOrganiser_Returns403()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEnrolment(ctx);
        ctx.Users.Add(TestHelpers.CreateUser(3, "Organiser"));
        await ctx.SaveChangesAsync();

        var controller = new ResultsController(ctx);
        TestHelpers.AttachUser(controller, 3, "Organiser");

        var result = await controller.Capture(new ResultDto
        {
            EnrolmentId = 1,
            FinishTime = TimeSpan.FromMinutes(42),
            Position = 5
        });

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Capture_AlreadyCaptured_Returns409()
    {
        var ctx = TestHelpers.CreateInMemoryDb(Guid.NewGuid().ToString());
        await SeedEnrolment(ctx);
        ctx.Results.Add(new Result
        {
            EnrolmentId = 1,
            FinishTime = TimeSpan.FromMinutes(40),
            Position = 3
        });
        await ctx.SaveChangesAsync();

        var controller = new ResultsController(ctx);
        TestHelpers.AttachUser(controller, 1, "Organiser");

        var result = await controller.Capture(new ResultDto
        {
            EnrolmentId = 1,
            FinishTime = TimeSpan.FromMinutes(42),
            Position = 5
        });

        Assert.IsType<ConflictObjectResult>(result);
    }
}
