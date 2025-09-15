using MiniServer.Core;

Console.WriteLine("Starting the ToDoList APP...");

var server = new HttpServer(8080);
await server.StartAsync();