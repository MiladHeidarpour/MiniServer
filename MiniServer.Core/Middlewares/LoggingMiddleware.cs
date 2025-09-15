using MiniServer.Core.Http;

namespace MiniServer.Core.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        Console.WriteLine($"--> [{DateTime.Now:HH:mm:ss}] Received {context.Request.Method} request for {context.Request.Path}");

        await next();

        Console.WriteLine($"<-- [{DateTime.Now:HH:mm:ss}] Sent {context.Response.StatusCode} response for {context.Request.Path}");
    }
}