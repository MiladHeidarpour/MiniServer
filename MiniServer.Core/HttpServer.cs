using MiniServer.Core.Http;
using MiniServer.Core.Middlewares;
using MiniServer.Core.Routing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace MiniServer.Core;

public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly int _port;
    private readonly MiddlewarePipeline _pipeline;

    public HttpServer(int port = 8080)
    {
        _port = port;
        _listener = new TcpListener(IPAddress.Any, _port);

        var router = new Router();
        router.RegisterRoutes(Assembly.GetEntryAssembly()!);

        _pipeline = new MiddlewarePipeline();
        _pipeline.Use<LoggingMiddleware>();
        _router = router;
    }

    private readonly Router _router;

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine($"Server started on http://localhost:{_port}");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, leaveOpen: true);

        var requestLine = await reader.ReadLineAsync();
        if (string.IsNullOrEmpty(requestLine)) return;

        var requestParts = requestLine.Split(' ');
        var request = new HttpRequest { Method = requestParts[0], Path = requestParts[1] };

        var response = new HttpResponse();
        var context = new HttpContext(request, response);

        var loggingMiddleware = new LoggingMiddleware();
        await loggingMiddleware.InvokeAsync(context, async () => {
            var routingMiddleware = new RoutingMiddleware(_router);
            await routingMiddleware.InvokeAsync(context, () => Task.CompletedTask);
        });

        var responseBytes = context.Response.GetResponseBytes();
        await stream.WriteAsync(responseBytes);
        client.Close();
    }
}