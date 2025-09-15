using MiniServer.Core.Attributes;
using MiniServer.Core.Http;
using MiniServer.Core.Routing;
using ToDoList.Models;

namespace ToDoList.Controllers;

public class TasksController : BaseController
{
    private static readonly List<TodoTask> _tasks = new()
    {
        new TodoTask { Id = 1, Title = "Learn about HTTP", IsCompleted = true },
        new TodoTask { Id = 2, Title = "Build a Router", IsCompleted = true },
        new TodoTask { Id = 3, Title = "Return JSON data", IsCompleted = false }
    };

    [Route("GET", "/tasks")]
    public Task GetAllTasks()
    {
        HttpContext.Response.WriteJson(_tasks);
        return Task.CompletedTask;
    }

    [Route("GET", "/tasks/{id}")]
    public Task GetTaskById([FromRoute] int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            HttpContext.Response.StatusCode = 404;
            HttpContext.Response.WriteJson(new { message = $"Task with ID {id} not found." });
        }
        else
        {
            HttpContext.Response.WriteJson(task);
        }

        return Task.CompletedTask;
    }
}
