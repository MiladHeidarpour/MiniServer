
namespace MiniServer.Core.Http;

public class HttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}
