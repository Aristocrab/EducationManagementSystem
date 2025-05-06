namespace EducationManagementSystem.Application.Features.Grades.Dtos;

public class NewSubjectGradeDto
{
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public decimal Value { get; set; }
    public DateTime IssuedAt { get; set; }
}