using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;

namespace EducationManagementSystem.Application.Features.Sessions.Dtos;

public class SessionDto
{
    public Guid Id { get; set; }
    public required StudentDto StudentName { get; set; }
    public required SubjectDto SubjectTitle { get; set; }
    public decimal Grade { get; set; }
    public DateTime Date { get; set; }
}