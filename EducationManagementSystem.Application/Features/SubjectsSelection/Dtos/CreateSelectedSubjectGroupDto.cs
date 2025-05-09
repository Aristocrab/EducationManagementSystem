namespace EducationManagementSystem.Application.Features.SubjectsSelection.Dtos;

public sealed class CreateSelectedSubjectGroupDto
{
    public required Guid SubjectId { get; init; }
    public int MaxStudents { get; init; } = 100;
}