using EducationManagementSystem.Application.Features.Certificates.Dtos;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace EducationManagementSystem.Application.Features.Certificates.Strategies;

public class PrometheusCertificateParser : ICertificateParser
{
    public async Task<CertificateParsedInfo> ParseAsync(string url)
    {
        await new BrowserFetcher().DownloadAsync();

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-setuid-sandbox"]
        });

        var page = await browser.NewPageAsync();

        await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
        await page.SetViewportAsync(new ViewPortOptions { Width = 1280, Height = 800 });

        await page.GoToAsync("https://certs.prometheus.org.ua/cert/535d611c4a25455ca924f857bb4bbbad", WaitUntilNavigation.Networkidle0);

        await Task.Delay(1000);

        var content = await page.GetContentAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(content);

        var nameNode = doc.DocumentNode.SelectSingleNode("//dt[contains(text(),'Студент')]/following-sibling::dd[1]/b");
        var courseNode = doc.DocumentNode.SelectSingleNode("//dt[contains(text(),'Навчальний курс')]/following-sibling::dd[1]/b");

        var name = nameNode?.InnerText.Trim()!;
        var courseName = courseNode?.InnerText.Trim()!;
        
        var parsedInfo = new CertificateParsedInfo
        {
            StudentName = name,
            IssuedAt = DateTime.Today, // todo
            CourseTitle = courseName,
            Link = url,
            Issuer = "Prometheus",
        };
        
        return parsedInfo;
    }
}