using Microsoft.AspNetCore.Http;

namespace RaceDay.Api.DTOs;

public class ProfilePictureUploadDto
{
    public IFormFile File { get; set; } = null!;
}
