using Aristocrab.AspNetCore.AppModules;
using Hangfire;
using Hangfire.Storage.SQLite;
using HangfireBasicAuthenticationFilter;
using EducationManagementSystem.Application.Features.Lessons;

namespace EducationManagementSystem.WebApi.Modules.Hangfire;

public class HangfireModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config =>
        {
            config.UseSQLiteStorage();
        });
        builder.Services.AddHangfireServer();

        builder.Services.AddTransient<ILessonsService, LessonsService>();
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization =
            [
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = app.Configuration["HangfireUsername"],
                    Pass = app.Configuration["HangfirePassword"],
                }
            ]
        });
        
        var recurringJobOptions = new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev")
        };
        
        RecurringJob.AddOrUpdate<ILessonsService>("reset-lessons-status", 
            x => x.CopyLessonsToNextWeek(), 
            Cron.Weekly(DayOfWeek.Monday, 0, 10), 
            recurringJobOptions);
    }
}