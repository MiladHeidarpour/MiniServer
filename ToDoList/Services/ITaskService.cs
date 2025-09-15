using ToDoList.DTOs;
using ToDoList.Models;

namespace ToDoList.Services;

public interface ITaskService
{
    List<TodoTask> GetAll();
    TodoTask? GetById(int id);
    TodoTask Create(CreateTaskRequest request);
    TodoTask? Update(int id, UpdateTaskRequest request);
    bool Delete(int id);
}