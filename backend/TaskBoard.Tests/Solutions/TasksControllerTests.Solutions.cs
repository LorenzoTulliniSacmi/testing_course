using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Services;
using TaskBoard.WebApi.Controllers;
using TaskBoard.WebApi.DTOs;
using TaskBoard.WebApi.Mapping;
using Xunit;

namespace TaskBoard.Tests.Solutions;

/// <summary>
/// Soluzioni Modulo 3: Unit Test Controller
/// NOTA: Questi test sono le soluzioni complete degli esercizi
/// </summary>
[Trait("Category", "Solution")]
public class TasksControllerTestsSolutions
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly TasksController _sut;

    public TasksControllerTestsSolutions()
    {
        _serviceMock = new Mock<ITaskService>();
        _sut = new TasksController(_serviceMock.Object);
        MappingConfig.Configure();
    }

    /// <summary>
    /// Esercizio 1: GetById restituisce NotFound quando task non esiste
    /// </summary>
    [Fact]
    public async Task GetById_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetByIdAsync("nonexistent"))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.GetById("nonexistent");

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var error = notFoundResult.Value.Should().BeOfType<ErrorDto>().Subject;
        error.Error.Should().Be("Task not found");
    }

    /// <summary>
    /// Esercizio 2: GetById restituisce Ok con task quando esiste
    /// </summary>
    [Fact]
    public async Task GetById_WhenTaskExists_ReturnsOkWithTask()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = "123",
            Title = "Test Task",
            Description = "Test Desc",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.GetByIdAsync("123"))
            .ReturnsAsync(task);

        // Act
        var result = await _sut.GetById("123");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Id.Should().Be("123");
        dto.Title.Should().Be("Test Task");
    }

    /// <summary>
    /// Esercizio 3: Create con titolo vuoto restituisce BadRequest
    /// </summary>
    [Fact]
    public async Task Create_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "",
            Description = "Description"
        };

        // Act
        var result = await _sut.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);

        var error = badRequestResult.Value.Should().BeOfType<ErrorDto>().Subject;
        error.Error.Should().Be("Title is required");
    }

    /// <summary>
    /// Esercizio 4: Create con dati validi restituisce CreatedAtAction
    /// </summary>
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "New Description",
            Priority = "high"
        };

        var createdTask = new TaskItem
        {
            Id = "new-id",
            Title = "New Task",
            Description = "New Description",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.High,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.CreateAsync("New Task", "New Description", "high"))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _sut.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(TasksController.GetById));

        var dto = createdResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Id.Should().Be("new-id");
    }

    /// <summary>
    /// Esercizio 5: Delete quando task esiste restituisce NoContent
    /// </summary>
    [Fact]
    public async Task Delete_WhenTaskExists_ReturnsNoContent()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.DeleteAsync("123"))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Delete("123");

        // Assert
        result.Should().BeOfType<NoContentResult>();
        (result as NoContentResult)!.StatusCode.Should().Be(204);
    }

    /// <summary>
    /// Esercizio 6: Delete quando task non esiste restituisce NotFound
    /// </summary>
    [Fact]
    public async Task Delete_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.DeleteAsync("nonexistent"))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Delete("nonexistent");

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);

        var error = notFoundResult.Value.Should().BeOfType<ErrorDto>().Subject;
        error.Error.Should().Be("Task not found");
    }
}
