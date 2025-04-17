using Aristocrab.AspNetCore.AppModules;

namespace EducationManagementSystem.WebApi.Modules.Configuration;

public class ConfigurationModule : AppModule
{
    public override int OrderIndex => -2;

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
#if DEBUG
        builder.Configuration.AddYamlFile("appsettings.development.yaml", optional: false);
#else
        builder.Configuration.AddYamlFile("appsettings.yaml", optional: false);
#endif
    }
}