using MiniServer.Core.Http;

namespace MiniServer.Core.Routing;

public abstract class BaseController
{
    protected HttpContext HttpContext { get; private set; }

    public void SetContext(HttpContext context)
    {
        HttpContext = context;
    }
}
