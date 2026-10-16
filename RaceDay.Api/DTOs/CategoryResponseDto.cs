namespace RaceDay.Api.DTOs;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public double Distance { get; set; }
}