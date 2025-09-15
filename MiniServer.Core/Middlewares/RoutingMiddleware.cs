using MiniServer.Core.Http;
using MiniServer.Core.Routing;

namespace MiniServer.Core.Middlewares;

public class RoutingMiddleware : IMiddleware
{
    private readonly Router _router;

    public RoutingMiddleware(Router router)
    {
        _router = router;
    }

    public async Task InvokeAsync(HttpContext context, Func<Task> next)
    {
        await _router.RouteRequestAsync(context);
    }
}