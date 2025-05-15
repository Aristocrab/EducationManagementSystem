using EducationManagementSystem.Application.Features.Lessons.Dtos;

namespace EducationManagementSystem.Application.Features.Students.Dtos;

public class StudentDto
{
    public required Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string GroupId { get; init; }

    public List<LessonDto> Lessons { get; set; } = [];
}