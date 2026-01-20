using System.ComponentModel.DataAnnotations;

namespace TaskBoard.WebApi.DTOs;

public class CreateTaskDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Priority { get; set; }
}
