namespace EducationManagementSystem.Application.Features.Lessons.Dtos;

public record NewLessonDto
{
    public required string? StudentName { get; init; }
    public required Guid? ExistingStudentId { get; init; }
    public required DateTime DateTime { get; init; }
    public required string Description { get; init; }
    public required int DurationMinutes { get; init; }
}