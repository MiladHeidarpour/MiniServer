
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

        var responseText = "Hello from your own Server!";
        var response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {responseText.Length}\r\n\r\n{responseText}";
        var responseBytes = Encoding.UTF8.GetBytes(response);

        await stream.WriteAsync(responseBytes);

        client.Close();
    }
}
