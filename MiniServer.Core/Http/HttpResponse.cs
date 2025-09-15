using System.Text;
using System.Text.Json;

namespace MiniServer.Core.Http;

public class HttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string StatusMessage { get; set; } = "OK";
    public Dictionary<string, string> Headers { get; set; } = new();
    private readonly MemoryStream _body = new();

    public void Write(string content)
    {
        var buffer = Encoding.UTF8.GetBytes(content);
        _body.Write(buffer, 0, buffer.Length);
    }
    public void WriteJson(object data)
    {
        Headers["Content-Type"] = "application/json";
        var jsonString = JsonSerializer.Serialize(data);
        Write(jsonString);
    }
    internal byte[] GetResponseBytes()
    {
        var responseBuilder = new StringBuilder();
        responseBuilder.Append($"HTTP/1.1 {StatusCode} {StatusMessage}\r\n");

        if (!Headers.ContainsKey("Content-Length"))
        {
            Headers["Content-Length"] = _body.Length.ToString();
        }

        foreach (var header in Headers)
        {
            responseBuilder.Append($"{header.Key}: {header.Value}\r\n");
        }

        responseBuilder.Append("\r\n");

        var headerBytes = Encoding.UTF8.GetBytes(responseBuilder.ToString());
        var responseBytes = new byte[headerBytes.Length + _body.Length];

        Buffer.BlockCopy(headerBytes, 0, responseBytes, 0, headerBytes.Length);
        Buffer.BlockCopy(_body.ToArray(), 0, responseBytes, headerBytes.Length, (int)_body.Length);

        return responseBytes;
    }
}