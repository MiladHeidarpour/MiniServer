using MiniServer.Core.Http;

namespace MiniServer.Core.Middlewares;

public class MiddlewarePipeline
{
    private readonly List<IMiddleware> _middlewares = new();

    public void Use(IMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public async Task ExecuteAsync(HttpContext context)
    {
        var index = 0;
        async Task Next()
        {
            if (index < _middlewares.Count)
            {
                var middleware = _middlewares[index++];
                await middleware.InvokeAsync(context, Next);
            }
        }
        await Next();
    }
}