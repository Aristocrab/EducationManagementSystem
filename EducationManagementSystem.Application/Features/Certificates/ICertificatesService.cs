using EducationManagementSystem.Application.Features.Certificates.Dtos;
using EducationManagementSystem.Application.Shared.Auth.Models;

namespace EducationManagementSystem.Application.Features.Certificates;

public interface ICertificatesService
{
    Task<IReadOnlyList<CertificateDto>> GetAll(User currentUser);
    Task<CertificateDto> GetById(Guid id);
    Task UploadCertificate(NewCertificateDto dto, User currentUser);
    Task UploadCertificateManually(ManualCertificateDto dto, User currentUser);
    Task AddAllowedCertificate(AllowedCertificateDto dto, User currentUser);
    Task Delete(Guid id, User currentUser);
}