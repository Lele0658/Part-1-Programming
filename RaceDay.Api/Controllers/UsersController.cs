using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Services;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public UsersController(RaceDayDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _context.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty
        });
    }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();

        user.FullName = dto.FullName;
        await _context.SaveChangesAsync();

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty
        });
    }

    [HttpPost("profile-picture")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] ProfilePictureUploadDto dto, [FromServices] BlobService blobService)
    {
        var user = await _context.Users.FindAsync(CurrentUserId);
        if (user == null) return NotFound();

        if (dto.File == null || dto.File.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var url = await blobService.UploadFileAsync(dto.File);
        user.ProfilePictureUrl = url;
        await _context.SaveChangesAsync();

        return Ok(new { profilePictureUrl = url });
    }

}