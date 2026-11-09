using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(RaceDayDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        if (dto.Role != "Organiser" && dto.Role != "Participant")
            return BadRequest(new { message = "Role must be 'Organiser' or 'Participant'." });

        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
            return Conflict(new { message = "A user with this email already exists." });

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);

        return StatusCode(StatusCodes.Status201Created, new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password." });

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid)
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        });
    }
}