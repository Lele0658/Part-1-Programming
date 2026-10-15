namespace RaceDay.Api.Models;

public class Event
{
    public int Id { get; set; }
    public int OrganiserId { get; set; }
    public User Organiser { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Distance { get; set; }
    public string EventType { get; set; } = string.Empty; // "run", "walk", "cycle"

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}