namespace EducationManagementSystem.Application.Features.EducationEvents.Dtos;

public class EducationEventDto
{
    public required Guid Id { get; set; }
    public required string EventType { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}