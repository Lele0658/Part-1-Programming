using Microsoft.AspNetCore.Http;

namespace RaceDay.Api.DTOs;

public class BannerUploadDto
{
    public IFormFile File { get; set; } = null!;
}