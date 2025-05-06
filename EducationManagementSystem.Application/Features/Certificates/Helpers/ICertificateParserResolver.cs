using EducationManagementSystem.Application.Features.Certificates.Strategies;

namespace EducationManagementSystem.Application.Features.Certificates.Helpers;

public interface ICertificateParserResolver
{
    ICertificateParser Resolve(string url);
}