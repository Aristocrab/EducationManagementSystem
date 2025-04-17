using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Features.Payments.Interfaces;
using Flurl.Http;
using Refit;

namespace EducationManagementSystem.WebApi.Modules.Monobank;

public class MonobankModule : AppModule
{
    public override int OrderIndex => 1;
    
    private const string MonobankApiUrl = "https://api.monobank.ua";

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            try
            {
                var flurlResponse = MonobankApiUrl
                    .WithHeader("X-Token", builder.Configuration["MonobankApiKey"])
                    .PostJsonAsync(new
                    {
                        webHookUrl = builder.Configuration["WebhookUrl"]
                    })
                    .Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            try
            {
                var flurlResponse2 = MonobankApiUrl
                    .WithHeader("X-Token", builder.Configuration["MonobankOldApiKey"])
                    .PostJsonAsync(new
                    {
                        webHookUrl = builder.Configuration["WebhookUrl"]
                    })
                    .Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        builder.Services.AddScoped<IPaymentLinkApi>(_ => 
            RestService.For<IPaymentLinkApi>(MonobankApiUrl));
    }
}