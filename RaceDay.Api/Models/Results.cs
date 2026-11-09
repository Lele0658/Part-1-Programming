namespace RaceDay.Api.Models;

public class Result
{
    public int Id { get; set; }
    public int EnrolmentId { get; set; }
    public Enrolment Enrolment { get; set; } = null!;

    public TimeSpan FinishTime { get; set; }
    public int Position { get; set; }
}