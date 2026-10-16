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
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public EnrolmentsController(RaceDayDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(typeof(EnrolmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enrol([FromBody] EnrolmentDto dto)
    {
        var ev = await _context.Events.FindAsync(dto.EventId);
        if (ev == null) return NotFound(new { message = "Event not found." });

        var cat = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.EventId == dto.EventId);
        if (cat == null) return NotFound(new { message = "Category not found for this event." });

        var already = await _context.Enrolments
            .AnyAsync(e => e.ParticipantId == CurrentUserId && e.EventId == dto.EventId);
        if (already)
            return Conflict(new { message = "You are already enrolled in this event." });

        var enrolment = new Enrolment
        {
            ParticipantId = CurrentUserId,
            EventId = dto.EventId,
            CategoryId = dto.CategoryId
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        await _context.Entry(enrolment).Reference(e => e.Participant).LoadAsync();
        await _context.Entry(enrolment).Reference(e => e.Event).LoadAsync();
        await _context.Entry(enrolment).Reference(e => e.Category).LoadAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(enrolment));
    }

    [HttpGet("my")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(typeof(IEnumerable<EnrolmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyEnrolments()
    {
        var list = await _context.Enrolments
            .Where(e => e.ParticipantId == CurrentUserId)
            .Include(e => e.Participant)
            .Include(e => e.Event)
            .Include(e => e.Category)
            .ToListAsync();

        return Ok(list.Select(ToDto));
    }

    [HttpGet("event/{eventId}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(IEnumerable<EnrolmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EventEnrolments(int eventId)
    {
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        var list = await _context.Enrolments
            .Where(e => e.EventId == eventId)
            .Include(e => e.Participant)
            .Include(e => e.Event)
            .Include(e => e.Category)
            .ToListAsync();

        return Ok(list.Select(ToDto));
    }

    private static EnrolmentResponseDto ToDto(Enrolment e) => new()
    {
        Id = e.Id,
        ParticipantId = e.ParticipantId,
        ParticipantName = e.Participant?.FullName ?? string.Empty,
        EventId = e.EventId,
        EventName = e.Event?.Name ?? string.Empty,
        CategoryId = e.CategoryId,
        CategoryName = e.Category?.Name ?? string.Empty,
        EnrolledDate = e.EnrolledDate
    };
}