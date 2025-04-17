using System.Net;
using System.Text.Json;
using Aristocrab.AspNetCore.AppModules;
using EducationManagementSystem.Core.Exceptions;
using FluentValidation;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EducationManagementSystem.WebApi.Modules.ExceptionsHandling;

public class ExceptionsHandlingModule : AppModule
{
    public override int OrderIndex => -1;

    public override void ConfigureApplication(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch(Exception exception)
            {
                await HandleException(context, exception, app.Logger);
            }
        });
    }
    
    private static async Task HandleException(HttpContext context, Exception exception, ILogger logger)
    {
        var code = exception switch
        {
            UnauthorizedException or WrongPasswordException => HttpStatusCode.Unauthorized,
            NotFoundException => HttpStatusCode.NotFound,
            UserAlreadyExistsException or LessonsOverlapException => HttpStatusCode.Conflict,
            ArgumentNullException or ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;
        
        // Use LINQ inner exception
        if (exception is InvalidOperationException invalidOperationException)
        {
            exception = invalidOperationException.InnerException ?? invalidOperationException;
        }

        var errorMessage = exception switch
        {
            ValidationException validationException => validationException.Errors.First().ErrorMessage,
            _ => exception.Message
        };

        if (errorMessage.StartsWith("Cannot pass null model to Validate"))
        {
            errorMessage = "Validation failed";
        }

        var error = new
        {
            StatusCode = code,
            ErrorMessage = errorMessage
        };
        
        logger.LogError(exception, "{RequestMethod} {RequestPath} responded {StatusCode}. {ErrorMessage}",
            context.Request.Method, 
            context.Request.Path, 
            context.Response.StatusCode, 
            error.ErrorMessage);
        
        var errorJson = JsonSerializer.Serialize(error);
        await context.Response.WriteAsync(errorJson);
    }
}