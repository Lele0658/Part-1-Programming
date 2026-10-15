namespace RaceDay.Api.Models;

public class Enrolment
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }
    public User Participant { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;

    public Result? Result { get; set; }
}