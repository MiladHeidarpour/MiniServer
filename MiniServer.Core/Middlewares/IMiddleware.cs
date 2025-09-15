using MiniServer.Core.Http;

namespace MiniServer.Core.Middlewares;

public interface IMiddleware
{
    Task InvokeAsync(HttpContext context, Func<Task> next);
}
