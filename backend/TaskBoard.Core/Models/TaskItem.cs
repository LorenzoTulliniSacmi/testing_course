using TaskBoard.Core.Models.Enums;

namespace TaskBoard.Core.Models;

public class TaskItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public KanbanStatus Status { get; set; } = KanbanStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public bool Archived { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
