namespace EducationManagementSystem.Application.Features.Students.Dtos;

public class NewStudentDto
{
    public required string FullName { get; init; }
    public required List<string> Languages { get; init; }
    public required string GroupId { get; init; }
}