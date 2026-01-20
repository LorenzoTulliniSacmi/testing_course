using TaskBoard.Core.Models;
using TaskBoard.Core.Ports;

namespace TaskBoard.Core.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllAsync(TaskQueryParams query);
    Task<TaskItem?> GetByIdAsync(string id);
    Task<TaskItem> CreateAsync(string title, string description, string? priority);
    Task<TaskItem?> UpdateAsync(string id, TaskItem updates);
    Task<TaskItem?> PatchAsync(string id, Dictionary<string, object?> updates);
    Task<bool> DeleteAsync(string id);
}
