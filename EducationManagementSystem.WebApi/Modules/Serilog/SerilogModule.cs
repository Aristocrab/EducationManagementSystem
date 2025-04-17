using Aristocrab.AspNetCore.AppModules;
using Serilog;
using Serilog.Events;

namespace EducationManagementSystem.WebApi.Modules.Serilog;

public class SerilogModule : AppModule
{
    public override int OrderIndex => -1;

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}")
            .Enrich.FromLogContext()
            .CreateLogger();
        
        builder.Host.UseSerilog(logger);
    }
}