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
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public CategoriesController(RaceDayDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("event/{eventId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        var cats = await _context.Categories
            .Where(c => c.EventId == eventId)
            .ToListAsync();

        return Ok(cats.Select(ToDto));
    }

    [HttpPost("event/{eventId}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int eventId, [FromBody] CategoryDto dto)
    {
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound(new { message = "Event not found." });
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        var cat = new Category
        {
            EventId = eventId,
            Name = dto.Name,
            AgeGroup = dto.AgeGroup,
            Distance = dto.Distance
        };

        _context.Categories.Add(cat);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetForEvent), new { eventId }, ToDto(cat));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
    {
        var cat = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) return NotFound();
        if (cat.Event.OrganiserId != CurrentUserId) return Forbid();

        cat.Name = dto.Name;
        cat.AgeGroup = dto.AgeGroup;
        cat.Distance = dto.Distance;

        await _context.SaveChangesAsync();
        return Ok(ToDto(cat));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) return NotFound();
        if (cat.Event.OrganiserId != CurrentUserId) return Forbid();

        _context.Categories.Remove(cat);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static CategoryResponseDto ToDto(Category c) => new()
    {
        Id = c.Id,
        EventId = c.EventId,
        Name = c.Name,
        AgeGroup = c.AgeGroup,
        Distance = c.Distance
    };
}