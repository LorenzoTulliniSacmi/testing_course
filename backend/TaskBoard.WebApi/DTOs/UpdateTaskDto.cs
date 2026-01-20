using System.ComponentModel.DataAnnotations;

namespace TaskBoard.WebApi.DTOs;

public class UpdateTaskDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = string.Empty;

    public bool? Archived { get; set; }
}
