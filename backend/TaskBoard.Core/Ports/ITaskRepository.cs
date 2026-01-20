using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;

namespace TaskBoard.Core.Ports;

public record TaskQueryParams(
    KanbanStatus? Status = null,
    TaskPriority? Priority = null,
    string? Archived = null,
    string? Search = null,
    string? OrderBy = null,
    string? Order = null
);

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(TaskQueryParams query);
    Task<TaskItem?> GetByIdAsync(string id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(string id, TaskItem task);
    Task<bool> DeleteAsync(string id);
}
