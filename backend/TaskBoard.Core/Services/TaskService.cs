using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;

namespace TaskBoard.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(TaskQueryParams query)
    {
        return await _repository.GetAllAsync(query);
    }

    public async Task<TaskItem?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TaskItem> CreateAsync(string title, string description, string? priority)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Description = description,
            Status = KanbanStatus.Todo,
            Priority = ParsePriority(priority),
            Archived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(task);
    }

    public async Task<TaskItem?> UpdateAsync(string id, TaskItem updates)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Title = updates.Title;
        existing.Description = updates.Description;
        existing.Status = updates.Status;
        existing.Priority = updates.Priority;
        existing.Archived = updates.Archived;
        existing.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(id, existing);
    }

    public async Task<TaskItem?> PatchAsync(string id, Dictionary<string, object?> updates)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        foreach (var (key, value) in updates)
        {
            switch (key.ToLower())
            {
                case "title" when value is string title:
                    existing.Title = title;
                    break;
                case "description" when value is string desc:
                    existing.Description = desc;
                    break;
                case "status" when value is string status:
                    existing.Status = ParseStatus(status);
                    break;
                case "priority" when value is string priority:
                    existing.Priority = ParsePriority(priority);
                    break;
                case "archived" when value is bool archived:
                    existing.Archived = archived;
                    break;
            }
        }

        existing.UpdatedAt = DateTime.UtcNow;
        return await _repository.UpdateAsync(id, existing);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static TaskPriority ParsePriority(string? priority) => priority?.ToLower() switch
    {
        "low" => TaskPriority.Low,
        "high" => TaskPriority.High,
        _ => TaskPriority.Medium
    };

    private static KanbanStatus ParseStatus(string status) => status.ToLower() switch
    {
        "in-progress" => KanbanStatus.InProgress,
        "done" => KanbanStatus.Done,
        _ => KanbanStatus.Todo
    };
}
