using Mapster;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using TaskBoard.Core.Services;
using TaskBoard.WebApi.DTOs;

namespace TaskBoard.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all tasks",
        Description = "Retrieve tasks with optional filtering, searching, and ordering")]
    [SwaggerResponse(200, "List of tasks", typeof(IEnumerable<TaskDto>))]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? archived = "false",
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] string? order = null)
    {
        var query = new TaskQueryParams(
            Status: ParseStatus(status),
            Priority: ParsePriority(priority),
            Archived: archived,
            Search: search,
            OrderBy: orderBy,
            Order: order
        );

        var tasks = await _taskService.GetAllAsync(query);
        var dtos = tasks.Select(t => t.Adapt<TaskDto>());
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get task by ID")]
    [SwaggerResponse(200, "Task found", typeof(TaskDto))]
    [SwaggerResponse(404, "Task not found", typeof(ErrorDto))]
    public async Task<ActionResult<TaskDto>> GetById(string id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound(new ErrorDto { Error = "Task not found" });

        return Ok(task.Adapt<TaskDto>());
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new task")]
    [SwaggerResponse(201, "Task created", typeof(TaskDto))]
    [SwaggerResponse(400, "Validation error", typeof(ErrorDto))]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest(new ErrorDto { Error = "Title is required" });

        var task = await _taskService.CreateAsync(dto.Title, dto.Description, dto.Priority);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task.Adapt<TaskDto>());
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Full update of a task",
        Description = "All fields (title, description, status, priority) are required")]
    [SwaggerResponse(200, "Task updated", typeof(TaskDto))]
    [SwaggerResponse(400, "Validation error", typeof(ErrorDto))]
    [SwaggerResponse(404, "Task not found", typeof(ErrorDto))]
    public async Task<ActionResult<TaskDto>> Update(string id, [FromBody] UpdateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) ||
            string.IsNullOrWhiteSpace(dto.Description) ||
            string.IsNullOrWhiteSpace(dto.Status) ||
            string.IsNullOrWhiteSpace(dto.Priority))
        {
            return BadRequest(new ErrorDto
            {
                Error = "All fields (title, description, status, priority) are required for PUT"
            });
        }

        var taskItem = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = ParseStatusRequired(dto.Status),
            Priority = ParsePriorityRequired(dto.Priority),
            Archived = dto.Archived ?? false
        };

        var updated = await _taskService.UpdateAsync(id, taskItem);
        if (updated == null)
            return NotFound(new ErrorDto { Error = "Task not found" });

        return Ok(updated.Adapt<TaskDto>());
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Partial update of a task",
        Description = "Update any subset of fields. Use this to archive/unarchive tasks.")]
    [SwaggerResponse(200, "Task updated", typeof(TaskDto))]
    [SwaggerResponse(404, "Task not found", typeof(ErrorDto))]
    public async Task<ActionResult<TaskDto>> Patch(string id, [FromBody] PatchTaskDto dto)
    {
        var updates = new Dictionary<string, object?>();

        if (dto.Title != null) updates["title"] = dto.Title;
        if (dto.Description != null) updates["description"] = dto.Description;
        if (dto.Status != null) updates["status"] = dto.Status;
        if (dto.Priority != null) updates["priority"] = dto.Priority;
        if (dto.Archived.HasValue) updates["archived"] = dto.Archived.Value;

        var updated = await _taskService.PatchAsync(id, updates);
        if (updated == null)
            return NotFound(new ErrorDto { Error = "Task not found" });

        return Ok(updated.Adapt<TaskDto>());
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a task")]
    [SwaggerResponse(204, "Task deleted")]
    [SwaggerResponse(404, "Task not found", typeof(ErrorDto))]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _taskService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new ErrorDto { Error = "Task not found" });

        return NoContent();
    }

    private static KanbanStatus? ParseStatus(string? status) => status?.ToLower() switch
    {
        "todo" => KanbanStatus.Todo,
        "in-progress" => KanbanStatus.InProgress,
        "done" => KanbanStatus.Done,
        _ => null
    };

    private static TaskPriority? ParsePriority(string? priority) => priority?.ToLower() switch
    {
        "low" => TaskPriority.Low,
        "medium" => TaskPriority.Medium,
        "high" => TaskPriority.High,
        _ => null
    };

    private static KanbanStatus ParseStatusRequired(string status) => status.ToLower() switch
    {
        "in-progress" => KanbanStatus.InProgress,
        "done" => KanbanStatus.Done,
        _ => KanbanStatus.Todo
    };

    private static TaskPriority ParsePriorityRequired(string priority) => priority.ToLower() switch
    {
        "low" => TaskPriority.Low,
        "high" => TaskPriority.High,
        _ => TaskPriority.Medium
    };
}
