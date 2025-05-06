using EducationManagementSystem.Application.Features.Certificates.Strategies;

namespace EducationManagementSystem.Application.Features.Certificates.Helpers;

public class CertificateParserResolver : ICertificateParserResolver
{
    private readonly CourseraCertificateParser _coursera;
    private readonly PrometheusCertificateParser _prometheus;

    public CertificateParserResolver(CourseraCertificateParser coursera, PrometheusCertificateParser prometheus)
    {
        _coursera = coursera;
        _prometheus = prometheus;
    }

    public ICertificateParser Resolve(string url)
    {
        if (url.Contains("coursera"))
            return _coursera;

        if (url.Contains("prometheus"))
            return _prometheus;

        throw new ArgumentException("Unsupported certificate provider");
    }
}
