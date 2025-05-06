namespace EducationManagementSystem.Application.Features.Certificates.Dtos;

public class NewCertificateDto
{
    public Guid StudentId { get; set; }
    public Guid SubjectId { get; set; }
    public string Link { get; set; } = null!;
}