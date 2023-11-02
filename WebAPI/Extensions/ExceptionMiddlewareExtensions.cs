﻿using Entities.ErrorModel;
using Microsoft.AspNetCore.Diagnostics;
using Repositories.EFCore;
using System.Net;
using Entities.Exceptions;

namespace WebAPI.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app,
            ILoggerService logger)
        {
            app.UseExceptionHandler(appError=>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature=context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature is null)
                    {
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            _ => StatusCodes.Status500InternalServerError
                        };
                        logger.LogError($"Something went wrong: {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = (int)context.Response.StatusCode,
                            Message=contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
