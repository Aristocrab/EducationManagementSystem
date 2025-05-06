using System.Globalization;
using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Application.Shared.Auth.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EducationManagementSystem.WebApi.Modules.FluentValidation;

public class FluentValidationModule : AppModule
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = false; // todo
        });
        
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
    }
}