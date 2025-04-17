using EducationManagementSystem.Application.Features.Teachers.Dtos;

namespace EducationManagementSystem.Application.Features.Lessons.Dtos;

public class LessonDto
{
    public Guid Id { get; set; }
    public required StudentDtoShort Student { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required bool OneTime { get; set; }
    public required decimal TeacherEarnings { get; set; }
    public required string Status { get; set; }
    public required int DurationMinutes { get; set; }
    public required bool Paid { get; set; }
    public required DateTime DateTime { get; set; }
    public required TeacherDtoShort Teacher { get; set; }
}