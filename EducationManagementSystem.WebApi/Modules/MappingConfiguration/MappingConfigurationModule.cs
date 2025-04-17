using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Features.Lessons.Mappings;
using Mapster;

namespace EducationManagementSystem.WebApi.Modules.MappingConfiguration;

public class MappingConfigurationModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(LessonsMappings).Assembly);
    }
}