namespace EducationManagementSystem.Application.Features.Certificates.Dtos;

public class CertificateParsedInfo
{
    public required string CourseTitle { get; set; }
    public decimal? CourseGrade { get; set; }
    public required string Issuer { get; set; }
    public required string Link { get; set; }
    public required DateTime IssuedAt { get; set; }
    public required string StudentName { get; set; }
}