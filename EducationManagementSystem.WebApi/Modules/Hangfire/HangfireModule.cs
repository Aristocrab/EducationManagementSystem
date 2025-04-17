using Aristocrab.AspNetCore.AppModules;
using Hangfire;
using Hangfire.Storage.SQLite;
using HangfireBasicAuthenticationFilter;
using EducationManagementSystem.Application.Features.Lessons;
using EducationManagementSystem.Application.Features.Telegram;

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
        
        RecurringJob.AddOrUpdate<ITelegramService>("send-weekly-report", 
            x => x.SendWeeklyReport(), 
            Cron.Weekly(DayOfWeek.Sunday, 21),
            recurringJobOptions);
        
        RecurringJob.AddOrUpdate<ITelegramService>("send-monthly-report",
            x => x.SendMonthlyReport(),
            "0 21 28-31 * *",
            recurringJobOptions);
        
        RecurringJob.AddOrUpdate<ITelegramService>("send-yearly-report",
            x => x.SendYearlyReport(),
            Cron.Yearly(12, 31, 22),
            recurringJobOptions);
        
        RecurringJob.AddOrUpdate<ITelegramService>("backup-database",
            x => x.BackupDatabase(),
            Cron.Daily(23, 50),
            recurringJobOptions);
    }
}