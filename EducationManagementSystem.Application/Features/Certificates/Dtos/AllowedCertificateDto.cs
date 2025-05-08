namespace EducationManagementSystem.Application.Features.Certificates.Dtos;

public class AllowedCertificateDto
{
    public required string Name { get; set; }
    public required Guid SubjectId { get; set; }
}