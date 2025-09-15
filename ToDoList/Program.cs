using MiniServer.Core;
using System.Reflection;

Console.WriteLine("Starting the ToDoList APP...");

var server = new HttpServer(8080);
server.MapRoutes(Assembly.GetExecutingAssembly());

await server.StartAsync();