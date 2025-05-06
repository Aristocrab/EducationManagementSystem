namespace EducationManagementSystem.Application.Features.Certificates.Dtos;

public class CertificateDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = default!;
    public string SubjectTitle { get; set; } = default!;
    public string CourseTitle { get; set; } = default!;
    public decimal CourseGrade { get; set; }
    public decimal MaxGrade { get; set; }
    public string Issuer { get; set; } = default!;
    public string Link { get; set; } = default!;
    public DateTime IssuedAt { get; set; }
}