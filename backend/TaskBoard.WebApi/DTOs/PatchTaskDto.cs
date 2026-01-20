namespace TaskBoard.WebApi.DTOs;

public class PatchTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public bool? Archived { get; set; }
}
