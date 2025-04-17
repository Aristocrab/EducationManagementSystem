namespace EducationManagementSystem.Application.Features.Lessons.Dtos;

public class StudentDtoShort
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public string? MessengerLink { get; set; }
    public List<string> Languages { get; init; } = [];
}