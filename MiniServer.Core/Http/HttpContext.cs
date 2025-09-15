namespace MiniServer.Core.Http;

public class HttpContext
{
    public HttpRequest Request { get; }
    public HttpResponse Response { get; }

    public HttpContext(HttpRequest request, HttpResponse response)
    {
        Request = request;
        Response = response;
    }
}