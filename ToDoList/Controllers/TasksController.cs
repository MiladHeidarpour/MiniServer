using MiniServer.Core.Attributes;
using MiniServer.Core.Http;
using MiniServer.Core.Routing;
using ToDoList.DTOs;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers;

public class TasksController : BaseController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// دریافت لیست تمام کارها
    /// </summary>
    [Route("GET", "/tasks")]
    public Task GetAllTasks()
    {
        var tasks = _taskService.GetAll();
        HttpContext.Response.WriteJson(tasks);
        return Task.CompletedTask;
    }

    /// <summary>
    /// دریافت یک کار مشخص بر اساس شناسه (ID)
    /// </summary>
    [Route("GET", "/tasks/{id}")]
    public Task GetTaskById([FromRoute] int id)
    {
        var task = _taskService.GetById(id);

        if (task is null)
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

    /// <summary>
    /// ایجاد یک کار جدید
    /// </summary>
    [Route("POST", "/tasks")]
    public Task CreateTask([FromBody] CreateTaskRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Title))
        {
            HttpContext.Response.StatusCode = 400;
            HttpContext.Response.WriteJson(new { message = "Task title cannot be empty." });
            return Task.CompletedTask;
        }

        var newTask = _taskService.Create(request);

        HttpContext.Response.StatusCode = 201;
        HttpContext.Response.WriteJson(newTask);
        return Task.CompletedTask;
    }

    /// <summary>
    /// به‌روزرسانی یک کار موجود
    /// </summary>
    [Route("PUT", "/tasks/{id}")]
    public Task UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Title))
        {
            HttpContext.Response.StatusCode = 400;
            HttpContext.Response.WriteJson(new { message = "Task title cannot be empty." });
            return Task.CompletedTask;
        }

        var updatedTask = _taskService.Update(id, request);

        if (updatedTask is null)
        {
            HttpContext.Response.StatusCode = 404;
            HttpContext.Response.WriteJson(new { message = $"Task with ID {id} not found." });
        }
        else
        {
            HttpContext.Response.WriteJson(updatedTask);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// حذف یک کار
    /// </summary>
    [Route("DELETE", "/tasks/{id}")]
    public Task DeleteTask([FromRoute] int id)
    {
        var wasDeleted = _taskService.Delete(id);

        if (!wasDeleted)
        {
            HttpContext.Response.StatusCode = 404;
            HttpContext.Response.WriteJson(new { message = $"Task with ID {id} not found." });
        }
        else
        {
            HttpContext.Response.StatusCode = 204;
        }

        return Task.CompletedTask;
    }
}
