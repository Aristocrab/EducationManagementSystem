using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;

namespace EducationManagementSystem.Application.Features.Sessions.Dtos;

public class SessionDto
{
    public Guid Id { get; set; }
    public required StudentDto Student { get; set; }
    public required SubjectDto Subject { get; set; }
    public decimal Grade { get; set; }
    public DateTime Date { get; set; }
}