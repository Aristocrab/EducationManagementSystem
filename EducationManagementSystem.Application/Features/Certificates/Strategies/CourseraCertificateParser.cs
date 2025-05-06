using System.Text.RegularExpressions;
using EducationManagementSystem.Application.Features.Certificates.Dtos;

namespace EducationManagementSystem.Application.Features.Certificates.Strategies;

public class CourseraCertificateParser : ICertificateParser
{
    public async Task<CertificateParsedInfo> ParseAsync(string url)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        
        var nameMatch = Regex.Match(content, "Completed by <strong>([^<]+)</strong>");
        var name = nameMatch.Success ? nameMatch.Groups[1].Value.Trim() : "Not found";
        
        var dateMatch = Regex.Match(content, "<p><strong>([^<]+)</strong></p>");
        var date = dateMatch.Success ? dateMatch.Groups[1].Value.Trim() : "Not found";
        
        var courseMatch = Regex.Match(content, "completion of <a [^>]+>([^<]+)</a>");
        var courseName = courseMatch.Success ? courseMatch.Groups[1].Value.Trim() : "Not found";
        
        var parsedInfo = new CertificateParsedInfo
        {
            StudentName = name,
            IssuedAt = DateTime.Parse(date),
            CourseTitle = courseName,
            Link = url,
            Issuer = "Coursera"
        };
        
        return parsedInfo;
    }
}