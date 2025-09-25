using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace qlnv
{
    public static class GlobalExceptionHandler
    {
        public static void UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(a =>
            {
                a.Run(async context =>
                {
                    var logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("GlobalExceptionHandler");
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = feature?.Error;

                    var problem = new
                    {
                        message = "An unexpected error occurred.",
                        detail = ex?.Message
                    };

                    logger?.LogError(ex, "Unhandled exception: {Message}", ex?.Message);

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
                });
            });
        }
    }
}
