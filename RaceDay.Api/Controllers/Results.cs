using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public ResultsController(RaceDayDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(ResultResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Capture([FromBody] ResultDto dto)
    {
        var enrolment = await _context.Enrolments
            .Include(e => e.Event)
            .Include(e => e.Participant)
            .Include(e => e.Category)
            .Include(e => e.Result)
            .FirstOrDefaultAsync(e => e.Id == dto.EnrolmentId);

        if (enrolment == null) return NotFound(new { message = "Enrolment not found." });
        if (enrolment.Event.OrganiserId != CurrentUserId) return Forbid();
        if (enrolment.Result != null)
            return Conflict(new { message = "Result already captured for this enrolment." });

        var result = new Result
        {
            EnrolmentId = enrolment.Id,
            FinishTime = dto.FinishTime,
            Position = dto.Position
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(result, enrolment));
    }

    [HttpGet("my")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(typeof(IEnumerable<ResultResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyResults()
    {
        var list = await _context.Results
            .Include(r => r.Enrolment).ThenInclude(e => e.Participant)
            .Include(r => r.Enrolment).ThenInclude(e => e.Event)
            .Include(r => r.Enrolment).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.ParticipantId == CurrentUserId)
            .ToListAsync();

        return Ok(list.Select(r => ToDto(r, r.Enrolment)));
    }

    [HttpGet("event/{eventId}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(IEnumerable<ResultResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EventResults(int eventId)
    {
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        var list = await _context.Results
            .Include(r => r.Enrolment).ThenInclude(e => e.Participant)
            .Include(r => r.Enrolment).ThenInclude(e => e.Event)
            .Include(r => r.Enrolment).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.EventId == eventId)
            .OrderBy(r => r.Position)
            .ToListAsync();

        return Ok(list.Select(r => ToDto(r, r.Enrolment)));
    }

    private static ResultResponseDto ToDto(Result r, Enrolment e) => new()
    {
        Id = r.Id,
        EnrolmentId = r.EnrolmentId,
        ParticipantName = e.Participant?.FullName ?? string.Empty,
        EventName = e.Event?.Name ?? string.Empty,
        CategoryName = e.Category?.Name ?? string.Empty,
        FinishTime = r.FinishTime,
        Position = r.Position
    };
}