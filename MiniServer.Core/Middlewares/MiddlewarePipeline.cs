using MiniServer.Core.Http;

namespace MiniServer.Core.Middlewares;

public class MiddlewarePipeline
{
    private readonly List<Type> _middlewares = new();

    public void Use<T>() where T : IMiddleware
    {
        _middlewares.Add(typeof(T));
    }

    public async Task ExecuteAsync(HttpContext context)
    {
        var index = 0;

        async Task Next()
        {
            if (index < _middlewares.Count)
            {
                var middlewareType = _middlewares[index++];
                var middleware = (IMiddleware)Activator.CreateInstance(middlewareType)!;
                await middleware.InvokeAsync(context, Next);
            }
        }

        await Next();
    }
}
