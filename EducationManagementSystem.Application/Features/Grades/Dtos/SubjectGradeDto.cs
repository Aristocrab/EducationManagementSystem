using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;

namespace EducationManagementSystem.Application.Features.Grades.Dtos;

public class SubjectGradeDto
{
    public Guid Id { get; set; }
    public required StudentDto StudentName { get; set; } 
    public required SubjectDto SubjectTitle { get; set; } 
    public decimal Value { get; set; }
    public DateTime IssuedAt { get; set; }
}