namespace RaceDay.Api.DTOs;

public class ResultResponseDto
{
    public int Id { get; set; }
    public int EnrolmentId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public TimeSpan FinishTime { get; set; }
    public int Position { get; set; }
}