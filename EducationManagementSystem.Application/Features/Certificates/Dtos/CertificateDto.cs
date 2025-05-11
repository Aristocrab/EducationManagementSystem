using EducationManagementSystem.Application.Features.Students.Dtos;
using EducationManagementSystem.Application.Features.Subjects.Dtos;

namespace EducationManagementSystem.Application.Features.Certificates.Dtos;

public class CertificateDto
{
    public Guid Id { get; set; }
    public required StudentDto Student { get; set; } 
    public required SubjectDto Subject { get; set; }
    public string CourseTitle { get; set; } = default!;
    public decimal CourseGrade { get; set; }
    public string Issuer { get; set; } = default!;
    public string Link { get; set; } = default!;
    public DateTime IssuedAt { get; set; }
}