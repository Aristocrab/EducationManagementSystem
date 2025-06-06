﻿using System.Diagnostics;
using Aristocrab.AspNetCore.AppModules;

namespace EducationManagementSystem.WebApi.Modules.RequestLogging;

public class RequestLoggingModule : AppModule
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if(context.Request.Path.StartsWithSegments("/webhook"))
            {
                await next(context);
                return;
            }
            
            app.Logger.LogInformation("{RequestMethod} {RequestPath} started", 
                context.Request.Method, 
                context.Request.Path);
            
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            await next(context);
            stopwatch.Stop();
            
            app.Logger.LogInformation("{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000} ms",
                context.Request.Method, 
                context.Request.Path, 
                context.Response.StatusCode, 
                stopwatch.Elapsed.TotalMilliseconds);
        });
    }
}