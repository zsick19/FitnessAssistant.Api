using System;
using System.Diagnostics;

namespace FitnessAssistant.Api.Shared.Timing;

public class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            logger.LogInformation("{RequestedMethod} {RequestedPath} completed with status {Status} in {ElapsedTime}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds
            );
        }
    }

}
