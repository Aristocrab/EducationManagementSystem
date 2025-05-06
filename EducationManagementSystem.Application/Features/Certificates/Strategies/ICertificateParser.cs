using EducationManagementSystem.Application.Features.Certificates.Dtos;

namespace EducationManagementSystem.Application.Features.Certificates.Strategies;

public interface ICertificateParser
{
    Task<CertificateParsedInfo> ParseAsync(string url);
}