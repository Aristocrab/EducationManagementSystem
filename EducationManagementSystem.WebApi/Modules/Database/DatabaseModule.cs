using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Database;
using Microsoft.EntityFrameworkCore;

namespace EducationManagementSystem.WebApi.Modules.Database;

public class DatabaseModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        Directory.CreateDirectory("../Database/");
        
        builder.Services.AddScoped<DatabaseSeeder>();
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration["DatabaseConnection"]));
    }

    public override void ConfigureApplication(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var db = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        db.FillDb();
    }
}