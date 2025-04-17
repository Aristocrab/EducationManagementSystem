using EducationManagementSystem.Application.Features.Lessons.Dtos;

namespace EducationManagementSystem.Application.Features.Teachers.Dtos;

public class TeacherDto
{
    public Guid Id { get; set; }
    public string Role { get; set; } = default!;
    
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required decimal Balance { get; set; }
    public required DateTime RegisteredAt { get; set; }
    public required string WorkingHours { get; set; }
    public string? MessengerLink { get; set; }

    public List<LessonDto> Lessons { get; set; } = [];
}