using ToDoList.DTOs;
using ToDoList.Models;

namespace ToDoList.Services;

public class TaskService : ITaskService
{
    private static readonly List<TodoTask> _tasks = new()
    {
        new TodoTask { Id = 1, Title = "Learn about HTTP", IsCompleted = true },
        new TodoTask { Id = 2, Title = "Build a Router", IsCompleted = true },
        new TodoTask { Id = 3, Title = "Return JSON data", IsCompleted = false }
    };

    public List<TodoTask> GetAll() => _tasks;

    public TodoTask? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

    public TodoTask Create(CreateTaskRequest request)
    {
        var newId = _tasks.Any() ? _tasks.Max(t => t.Id) + 1 : 1;
        var newTask = new TodoTask
        {
            Id = newId,
            Title = request.Title,
            IsCompleted = false
        };
        _tasks.Add(newTask);
        return newTask;
    }

    public TodoTask? Update(int id, UpdateTaskRequest request)
    {
        var task = GetById(id);
        if (task == null) return null;

        task.Title = request.Title;
        task.IsCompleted = request.IsCompleted;
        return task;
    }

    public bool Delete(int id)
    {
        var task = GetById(id);
        if (task == null) return false;

        _tasks.Remove(task);
        return true;
    }
}