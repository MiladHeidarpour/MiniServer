
namespace MiniServer.Core.DI;

public class ServiceProvider
{
    private readonly Dictionary<Type, Func<object>> _transientFactories = new();
    private readonly Dictionary<Type, object> _singletonInstances = new();

    public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
    {
        var instance = (TInterface)Activator.CreateInstance(typeof(TImplementation))!;
        _singletonInstances[typeof(TInterface)] = instance;
    }

    public void RegisterTransient<TInterface, TImplementation>() where TImplementation : TInterface
    {
        _transientFactories[typeof(TInterface)] = () => Activator.CreateInstance(typeof(TImplementation))!;
    }

    public object GetService(Type type)
    {
        if (_singletonInstances.TryGetValue(type, out var singletonInstance))
        {
            return singletonInstance;
        }

        if (_transientFactories.TryGetValue(type, out var factory))
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
