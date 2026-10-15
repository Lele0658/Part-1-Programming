namespace RaceDay.Api.Models;

public class Category
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public string Name { get; set; } = string.Empty;   // e.g. "Senior 10km"
    public string AgeGroup { get; set; } = string.Empty; // e.g. "Under 20", "Open"
    public double Distance { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}