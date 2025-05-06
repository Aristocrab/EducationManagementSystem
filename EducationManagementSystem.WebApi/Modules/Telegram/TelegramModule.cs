using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Shared.Telegram.Interfaces;
using Refit;

namespace EducationManagementSystem.WebApi.Modules.Telegram;

public class TelegramModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITelegramApi>(_ =>
        {
            var token = builder.Configuration["TelegramBotToken"];
            var gitHubApi = RestService.For<ITelegramApi>($"https://api.telegram.org/bot{token}");
            
            return gitHubApi;
        });
    }
}