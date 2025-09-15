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

    public HttpServer(MiddlewarePipeline pipeline, int port = 8080)
    {
        _port = port;
        _listener = new TcpListener(IPAddress.Any, _port);
        _pipeline = pipeline;
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
        try
        {
            using var stream = client.GetStream();

            var requestLine = await ReadHttpLineAsync(stream);
            if (string.IsNullOrEmpty(requestLine)) return;

            var requestParts = requestLine.Split(' ');
            var request = new HttpRequest { Method = requestParts[0].ToUpper(), Path = requestParts[1] };

            string? headerLine;
            while (!string.IsNullOrEmpty(headerLine = await ReadHttpLineAsync(stream)))
            {
                var headerParts = headerLine.Split(':', 2);
                if (headerParts.Length == 2)
                {
                    request.Headers[headerParts[0].Trim()] = headerParts[1].Trim();
                }
            }

            if (request.Headers.TryGetValue("Content-Length", out var contentLengthStr) &&
                int.TryParse(contentLengthStr, out var contentLength) &&
                contentLength > 0)
            {
                var buffer = new byte[contentLength];
                var bytesRead = 0;
                while (bytesRead < contentLength)
                {
                    var read = await stream.ReadAsync(buffer, bytesRead, contentLength - bytesRead);
                    if (read == 0) break;
                    bytesRead += read;
                }
                await request.Body.WriteAsync(buffer, 0, bytesRead);
                request.Body.Position = 0;
            }

            var response = new HttpResponse();
            var context = new HttpContext(request, response);

            await _pipeline.ExecuteAsync(context);

            var responseBytes = context.Response.GetResponseBytes();
            await stream.WriteAsync(responseBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL ERROR in HandleClientAsync]: {ex}");
        }
        finally
        {
            client.Close();
        }
    }

    private async Task<string> ReadHttpLineAsync(NetworkStream stream)
    {
        var buffer = new List<byte>();
        while (true)
        {
            var byteRead = stream.ReadByte();
            if (byteRead == -1) break; // End of stream
            if (byteRead == '\n') break; // End of line
            if (byteRead != '\r')
            {
                buffer.Add((byte)byteRead);
            }
        }
        return System.Text.Encoding.UTF8.GetString(buffer.ToArray());
    }
}