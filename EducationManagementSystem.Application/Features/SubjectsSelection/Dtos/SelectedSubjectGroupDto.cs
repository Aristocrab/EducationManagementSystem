using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;

namespace EducationManagementSystem.Application.Features.SubjectsSelection.Dtos;

public sealed class SelectedSubjectGroupDto
{
    public required Guid Id { get; init; }
    public required SubjectDto Subject { get; init; }
    public required int MaxStudents { get; init; }
    public required List<StudentDto> Students { get; init; } = [];
}