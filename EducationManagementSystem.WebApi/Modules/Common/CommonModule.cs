using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Features.Certificates.Helpers;
using EducationManagementSystem.Application.Features.Certificates.Strategies;
using EducationManagementSystem.Application.Features.Teachers;

namespace EducationManagementSystem.WebApi.Modules.Common;

public class CommonModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        builder.Services.AddControllers();
        
        // Register services
        builder.Services.Scan(scan => scan
            .FromAssemblyOf<ITeachersService>()
            .AddClasses(classes => classes.Where(x => x.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        builder.Services.AddScoped<ICertificateParserResolver, CertificateParserResolver>();
        builder.Services.AddScoped<CourseraCertificateParser>();
        builder.Services.AddScoped<PrometheusCertificateParser>();
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseCors(policyBuilder =>
        {
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyOrigin();
            policyBuilder.AllowAnyMethod();
        });

        #if DEBUG
        app.MapGet("/", () => Results.Redirect("/swagger/index.html", true));
        #else
        app.MapGet("/", () => "🤡");
        #endif
        
        app.MapHealthChecks("/health");
        app.MapControllers();
    }
}