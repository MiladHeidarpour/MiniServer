using MiniServer.Core.Attributes;
using MiniServer.Core.Http;
using System.Reflection;

namespace MiniServer.Core.Routing;

public class Router
{
    private readonly Dictionary<string, MethodInfo> _routes = new();

    public void RegisterRoutes(Assembly assembly)
    {
        var controllerTypes = assembly.GetTypes().Where(t => typeof(BaseController).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var controllerType in controllerTypes)
        {
            var methods = controllerType.GetMethods()
                .Where(m => m.GetCustomAttributes<RouteAttribute>().Any());

            foreach (var method in methods)
            {
                var routeAttribute = method.GetCustomAttribute<RouteAttribute>()!;
                var routeKey = $"{routeAttribute.Method}:{routeAttribute.Path}";
                _routes[routeKey] = method;
                Console.WriteLine($"[Router] Mapped {routeKey} to {controllerType.Name}.{method.Name}");
            }
        }
    }

    public async Task RouteRequestAsync(HttpContext context)
    {
        var routeKey = $"{context.Request.Method}:{context.Request.Path}";

        if (_routes.TryGetValue(routeKey, out var method))
        {
            var controllerType = method.DeclaringType!;
            var controllerInstance = (BaseController)Activator.CreateInstance(controllerType)!;
            controllerInstance.SetContext(context);

            await (Task)method.Invoke(controllerInstance, null)!;
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.StatusMessage = "Not Found";
            context.Response.Write("Route not found.");
        }
    }
}
