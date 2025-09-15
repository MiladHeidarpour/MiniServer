
using MiniServer.Core.Http;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MiniServer.Core;

public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly int _port;

    public HttpServer(int port = 8080)
    {
        _port = port;
        _listener = new TcpListener(IPAddress.Any, _port);
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
        using var reader = new StreamReader(stream);

        var requestLine = await reader.ReadLineAsync();
        if (string.IsNullOrEmpty(requestLine)) return;

        var requestParts = requestLine.Split(' ');
        var request = new HttpRequest
        {
            Method = requestParts[0],
            Path = requestParts[1]
        };

        string? line;
        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
        {
            var headerParts = line.Split(':', 2);
            if (headerParts.Length == 2)
            {
                request.Headers[headerParts[0]] = headerParts[1].Trim();
            }
        }

        var response = new HttpResponse();
        var context = new HttpContext(request, response);

        if (context.Request.Path == "/")
        {
            context.Response.Write("Welcome to the homepage!");
        }
        else if (context.Request.Path == "/about")
        {
            context.Response.Write("This is the About page.");
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.StatusMessage = "Not Found";
            context.Response.Write("Page not found.");
        }
        var responseBytes = context.Response.GetResponseBytes();
        await stream.WriteAsync(responseBytes);

        client.Close();
    }
}
