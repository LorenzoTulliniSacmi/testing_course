using Mapster;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.WebApi.DTOs;

namespace TaskBoard.WebApi.Mapping;

public static class MappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<TaskItem, TaskDto>.NewConfig()
            .Map(dest => dest.Status, src => StatusToString(src.Status))
            .Map(dest => dest.Priority, src => PriorityToString(src.Priority))
            .Map(dest => dest.CreatedAt, src => src.CreatedAt.ToString("o"))
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt.ToString("o"));
    }

    private static string StatusToString(KanbanStatus status) => status switch
    {
        KanbanStatus.InProgress => "in-progress",
        KanbanStatus.Done => "done",
        _ => "todo"
    };

    private static string PriorityToString(TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "low",
        TaskPriority.High => "high",
        _ => "medium"
    };
}
