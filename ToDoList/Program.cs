using MiniServer.Core;
using MiniServer.Core.DI;
using MiniServer.Core.Routing;
using System.Reflection;
using ToDoList.Services;

Console.WriteLine("Starting the ToDoList APP...");

var services = new Dictionary<Type, Func<object>>();
services.Add(typeof(ITaskService), () => new TaskService());
var serviceProvider = new ServiceProvider(services);

var router = new Router(serviceProvider);
router.RegisterRoutes(Assembly.GetExecutingAssembly());

var server = new HttpServer(router, 8080);
await server.StartAsync();