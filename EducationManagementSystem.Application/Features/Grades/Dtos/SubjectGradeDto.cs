using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;

namespace EducationManagementSystem.Application.Features.Grades.Dtos;

public class SubjectGradeDto
{
    public Guid Id { get; set; }
    public required StudentDto Student { get; set; } 
    public required SubjectDto Subject { get; set; } 
    public decimal Value { get; set; }
    public DateTime IssuedAt { get; set; }
}