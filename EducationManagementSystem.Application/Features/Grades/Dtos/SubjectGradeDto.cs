namespace EducationManagementSystem.Application.Features.Grades.Dtos;

public class SubjectGradeDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = default!;
    public string SubjectTitle { get; set; } = default!;
    public decimal Value { get; set; }
    public DateTime IssuedAt { get; set; }
}