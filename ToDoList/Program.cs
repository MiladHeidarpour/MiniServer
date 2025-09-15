using MiniServer.Core;
using MiniServer.Core.DI;
using MiniServer.Core.Middlewares;
using MiniServer.Core.Routing;
using System.Reflection;
using ToDoList.Services;

Console.WriteLine("Starting the ToDoList APP...");

var serviceProvider = new ServiceProvider();
serviceProvider.RegisterSingleton<ITaskService, TaskService>();

var router = new Router(serviceProvider);
router.RegisterRoutes(Assembly.GetExecutingAssembly());

var pipeline = new MiddlewarePipeline();
pipeline.Use(new LoggingMiddleware());
pipeline.Use(new RoutingMiddleware(router));

var server = new HttpServer(pipeline, 8080);
await server.StartAsync();