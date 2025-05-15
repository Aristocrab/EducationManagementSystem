using Aristocrab.AspNetCore.AppModules;

namespace EducationManagementSystem.WebApi.Modules.Configuration;

public class ConfigurationModule : AppModule
{
    public override int OrderIndex => -2;

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Configuration.AddYamlFile("appsettings.yaml", optional: false);
    }
}