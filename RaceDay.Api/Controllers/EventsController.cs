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
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public EventsController(RaceDayDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<EventResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var events = await _context.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return Ok(events.Select(ToDto));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _context.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null) return NotFound();
        return Ok(ToDto(ev));
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] EventDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Event name is required." });

        var ev = new Event
        {
            OrganiserId = CurrentUserId,
            Name = dto.Name,
            Description = dto.Description,
            Date = dto.Date,
            Location = dto.Location,
            Distance = dto.Distance,
            EventType = dto.EventType
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();
        await _context.Entry(ev).Reference(e => e.Organiser).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ToDto(ev));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] EventDto dto)
    {
        var ev = await _context.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        ev.Name = dto.Name;
        ev.Description = dto.Description;
        ev.Date = dto.Date;
        ev.Location = dto.Location;
        ev.Distance = dto.Distance;
        ev.EventType = dto.EventType;

        await _context.SaveChangesAsync();
        return Ok(ToDto(ev));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static EventResponseDto ToDto(Event e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        Date = e.Date,
        Location = e.Location,
        Distance = e.Distance,
        EventType = e.EventType,
        OrganiserId = e.OrganiserId,
        OrganiserName = e.Organiser?.FullName ?? string.Empty,
        Categories = e.Categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            EventId = c.EventId,
            Name = c.Name,
            AgeGroup = c.AgeGroup,
            Distance = c.Distance
        }).ToList()
    };
}