namespace Sarc.DTOs;

public class CreateScheduleDto
{
    public string RoomId { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}
