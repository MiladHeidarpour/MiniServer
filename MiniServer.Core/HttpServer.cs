
using MiniServer.Core.Http;
using MiniServer.Core.Routing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace MiniServer.Core;

public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly int _port;
    private readonly Router _router;

    public HttpServer(int port = 8080)
    {
        _port = port;
        _listener = new TcpListener(IPAddress.Any, _port);
        _router = new Router();
    }
    public void MapRoutes(Assembly assembly)
    {
        _router.RegisterRoutes(assembly);
    }

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
        string? line;
        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
        {
            var headerParts = line.Split(':', 2);
            if (headerParts.Length == 2) request.Headers[headerParts[0]] = headerParts[1].Trim();
        }

        if (request.Headers.TryGetValue("Content-Length", out var contentLengthStr) && int.TryParse(contentLengthStr, out var contentLength))
        {
            var buffer = new char[contentLength];
            await reader.ReadBlockAsync(buffer, 0, contentLength);
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(buffer);
            await request.Body.WriteAsync(bodyBytes, 0, bodyBytes.Length);
            request.Body.Position = 0;
        }

        var response = new HttpResponse();
        var context = new HttpContext(request, response);

        await _router.RouteRequestAsync(context);

        var responseBytes = context.Response.GetResponseBytes();
        await stream.WriteAsync(responseBytes);
        client.Close();
    }
}
