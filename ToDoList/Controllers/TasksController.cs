using MiniServer.Core.Attributes;
using MiniServer.Core.Http;
using MiniServer.Core.Routing;

namespace ToDoList.Controllers;

public class TasksController : BaseController
{
    [Route("GET", "/tasks")]
    public Task GetAllTasks()
    {
        HttpContext.Response.Write("This will be a list of all tasks!");
        return Task.CompletedTask;
    }

    [Route("GET", "/tasks/1")]
    public Task GetTaskById()
    {
        HttpContext.Response.Write("This will be task with ID 1.");
        return Task.CompletedTask;
    }
}
