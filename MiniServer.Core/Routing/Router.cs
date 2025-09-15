using MiniServer.Core.Attributes;
using MiniServer.Core.Http;
using System.Reflection;

namespace MiniServer.Core.Routing;

public class RouteEntry
{
    public string Method { get; set; }
    public string Template { get; set; }
    public MethodInfo MethodInfo { get; set; }
}

public class Router
{
    private readonly List<RouteEntry> _routes = new();

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
                _routes.Add(new RouteEntry
                {
                    Method = routeAttribute.Method,
                    Template = routeAttribute.Path,
                    MethodInfo = method
                });
                Console.WriteLine($"[Router] Mapped {routeAttribute.Method}:{routeAttribute.Path} to {controllerType.Name}.{method.Name}");
            }
        }
    }

    public async Task RouteRequestAsync(HttpContext context)
    {
        foreach (var route in _routes)
        {
            if (route.Method != context.Request.Method) continue;

            var routeValues = MatchRoute(route.Template, context.Request.Path);
            if (routeValues != null)
            {
                var controllerType = route.MethodInfo.DeclaringType!;
                var controllerInstance = (BaseController)Activator.CreateInstance(controllerType)!;
                controllerInstance.SetContext(context);

                var methodParams = await PrepareParametersAsync(route.MethodInfo, context, routeValues);

                await (Task)route.MethodInfo.Invoke(controllerInstance, methodParams)!;
                return;
            }
        }

        context.Response.StatusCode = 404;
        context.Response.StatusMessage = "Not Found";
        context.Response.Write("Route not found.");
    }

    private Dictionary<string, object> MatchRoute(string template, string path)
    {
        var templateParts = template.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var pathParts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (templateParts.Length != pathParts.Length) return null;

        var routeValues = new Dictionary<string, object>();

        for (int i = 0; i < templateParts.Length; i++)
        {
            if (templateParts[i].StartsWith("{") && templateParts[i].EndsWith("}"))
            {
                var key = templateParts[i].Trim('{', '}');
                routeValues[key] = pathParts[i];
            }
            else if (templateParts[i] != pathParts[i])
            {
                return null;
            }
        }
        return routeValues;
    }

    private async Task<object[]> PrepareParametersAsync(MethodInfo method, HttpContext context, Dictionary<string, object> routeValues)
    {
        var parameters = method.GetParameters();
        var result = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            if (param.GetCustomAttribute<FromRouteAttribute>() != null && routeValues.TryGetValue(param.Name, out var value))
            {
                result[i] = Convert.ChangeType(value, param.ParameterType);
            }
            else if (param.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                result[i] = await BindFromBodyAsync(context, param.ParameterType);
            }
        }
        return result;
    }
    private async Task<object> BindFromBodyAsync(HttpContext context, Type targetType)
    {
        return await System.Text.Json.JsonSerializer.DeserializeAsync(context.Request.Body, targetType, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
