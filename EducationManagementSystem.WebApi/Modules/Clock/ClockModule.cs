using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Shared.Clock;

namespace EducationManagementSystem.WebApi.Modules.Clock;

public class ClockModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IClock, KyivTimeClock>();
    }
}