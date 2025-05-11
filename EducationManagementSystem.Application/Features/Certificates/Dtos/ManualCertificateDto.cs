namespace EducationManagementSystem.Application.Features.Certificates.Dtos;

public class ManualCertificateDto
{
    public required Guid StudentId { get; set; } 
    public required Guid SubjectId { get; set; }
    public required string CourseTitle { get; set; }
    public required decimal CourseGrade { get; set; }
    public required string Issuer { get; set; }
    public string? Link { get; set; } = null;
    public DateTime IssuedAt { get; set; }
}