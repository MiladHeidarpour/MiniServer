
namespace MiniServer.Core.DI;

public class ServiceProvider
{
    private readonly Dictionary<Type, Func<object>> _services = new();

    public ServiceProvider(Dictionary<Type, Func<object>> services)
    {
        _services = services;
    }

    public object GetService(Type type)
    {
        if (_services.TryGetValue(type, out var factory))
        {
            return factory();
        }

        var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
        var parameters = constructor.GetParameters()
            .Select(p => GetService(p.ParameterType))
            .ToArray();

        return Activator.CreateInstance(type, parameters);
    }
}
