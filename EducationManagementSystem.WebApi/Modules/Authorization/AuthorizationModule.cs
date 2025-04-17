using System.Text;
using Aristocrab.AspNetCore.AppModules;
using Microsoft.IdentityModel.Tokens;

namespace EducationManagementSystem.WebApi.Modules.Authorization;

public class AuthorizationModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(
            config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = builder.Configuration["JwtAudience"],
                    ValidIssuer = builder.Configuration["JwtIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"] ?? "")
                        ),
                    ValidateLifetime = false
                };
            });
        builder.Services.AddAuthorization();
    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}