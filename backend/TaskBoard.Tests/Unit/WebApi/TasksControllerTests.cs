using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using TaskBoard.Core.Services;
using TaskBoard.WebApi.Controllers;
using TaskBoard.WebApi.DTOs;
using TaskBoard.WebApi.Mapping;

namespace TaskBoard.Tests.Unit.WebApi;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly TasksController _sut;

    public TasksControllerTests()
    {
        _serviceMock = new Mock<ITaskService>();
        _sut = new TasksController(_serviceMock.Object);

        // Configure Mapster for tests
        MappingConfig.Configure();
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new()
            {
                Id = "1",
                Title = "Task 1",
                Description = "Desc 1",
                Status = KanbanStatus.Todo,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = "2",
                Title = "Task 2",
                Description = "Desc 2",
                Status = KanbanStatus.Done,
                Priority = TaskPriority.High,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<TaskQueryParams>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetAll(null, null, "false", null, null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dtos = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskDto>>().Subject;
        dtos.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoTasks()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<TaskQueryParams>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await _sut.GetAll(null, null, "false", null, null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dtos = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskDto>>().Subject;
        dtos.Should().BeEmpty();
    }

    [Theory]
    [InlineData("todo")]
    [InlineData("in-progress")]
    [InlineData("done")]
    public async Task GetAll_PassesStatusFilter(string status)
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<TaskQueryParams>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        await _sut.GetAll(status, null, "false", null, null, null);

        // Assert
        _serviceMock.Verify(s => s.GetAllAsync(It.Is<TaskQueryParams>(q => q.Status != null)), Times.Once);
    }

    #endregion

    #region GetById Tests

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
        var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Id.Should().Be("123");
        dto.Title.Should().Be("Test Task");
    }

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

    #endregion

    #region Create Tests

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
        createdResult.ActionName.Should().Be(nameof(_sut.GetById));

        var dto = createdResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Id.Should().Be("new-id");
        dto.Title.Should().Be("New Task");
    }

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

    [Fact]
    public async Task Create_WithWhitespaceTitle_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "   ",
            Description = "Description"
        };

        // Act
        var result = await _sut.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var error = badRequestResult.Value.Should().BeOfType<ErrorDto>().Subject;
        error.Error.Should().Be("Title is required");
    }

    #endregion

    #region Update (PUT) Tests

    [Fact]
    public async Task Update_WithValidData_ReturnsOkWithUpdatedTask()
    {
        // Arrange
        var updateDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Desc",
            Status = "done",
            Priority = "high",
            Archived = false
        };

        var updatedTask = new TaskItem
        {
            Id = "123",
            Title = "Updated Title",
            Description = "Updated Desc",
            Status = KanbanStatus.Done,
            Priority = TaskPriority.High,
            Archived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.UpdateAsync("123", It.IsAny<TaskItem>()))
            .ReturnsAsync(updatedTask);

        // Act
        var result = await _sut.Update("123", updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Title.Should().Be("Updated Title");
        dto.Status.Should().Be("done");
    }

    [Fact]
    public async Task Update_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateTaskDto
        {
            Title = "Title",
            Description = "Desc",
            Status = "todo",
            Priority = "medium"
        };

        _serviceMock
            .Setup(s => s.UpdateAsync("nonexistent", It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.Update("nonexistent", updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Theory]
    [InlineData("", "desc", "todo", "medium")]
    [InlineData("title", "", "todo", "medium")]
    [InlineData("title", "desc", "", "medium")]
    [InlineData("title", "desc", "todo", "")]
    public async Task Update_WithMissingRequiredFields_ReturnsBadRequest(
        string title, string description, string status, string priority)
    {
        // Arrange
        var updateDto = new UpdateTaskDto
        {
            Title = title,
            Description = description,
            Status = status,
            Priority = priority
        };

        // Act
        var result = await _sut.Update("123", updateDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var error = badRequestResult.Value.Should().BeOfType<ErrorDto>().Subject;
        error.Error.Should().Contain("required for PUT");
    }

    #endregion

    #region Patch Tests

    [Fact]
    public async Task Patch_WithValidData_ReturnsOkWithUpdatedTask()
    {
        // Arrange
        var patchDto = new PatchTaskDto
        {
            Title = "Patched Title"
        };

        var patchedTask = new TaskItem
        {
            Id = "123",
            Title = "Patched Title",
            Description = "Original Desc",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.PatchAsync("123", It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync(patchedTask);

        // Act
        var result = await _sut.Patch("123", patchDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Title.Should().Be("Patched Title");
    }

    [Fact]
    public async Task Patch_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        var patchDto = new PatchTaskDto { Title = "New Title" };

        _serviceMock
            .Setup(s => s.PatchAsync("nonexistent", It.IsAny<Dictionary<string, object?>>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.Patch("nonexistent", patchDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Patch_WithArchivedFlag_PassesCorrectValue()
    {
        // Arrange
        var patchDto = new PatchTaskDto { Archived = true };

        var patchedTask = new TaskItem
        {
            Id = "123",
            Archived = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Dictionary<string, object?>? capturedUpdates = null;

        _serviceMock
            .Setup(s => s.PatchAsync("123", It.IsAny<Dictionary<string, object?>>()))
            .Callback<string, Dictionary<string, object?>>((id, updates) => capturedUpdates = updates)
            .ReturnsAsync(patchedTask);

        // Act
        await _sut.Patch("123", patchDto);

        // Assert
        capturedUpdates.Should().NotBeNull();
        capturedUpdates!.Should().ContainKey("archived");
        capturedUpdates["archived"].Should().Be(true);
    }

    #endregion

    #region Delete Tests

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
    }

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
        var error = notFoundResult.Value.Should().BeOfType<ErrorDto>().Subject;
        error.Error.Should().Be("Task not found");
    }

    [Fact]
    public async Task Delete_CallsServiceWithCorrectId()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _sut.Delete("specific-id");

        // Assert
        _serviceMock.Verify(s => s.DeleteAsync("specific-id"), Times.Once);
    }

    #endregion

    #region Status and Priority Mapping Tests

    [Theory]
    [InlineData(KanbanStatus.Todo, "todo")]
    [InlineData(KanbanStatus.InProgress, "in-progress")]
    [InlineData(KanbanStatus.Done, "done")]
    public async Task GetById_MapsStatusCorrectly(KanbanStatus status, string expected)
    {
        // Arrange
        var task = new TaskItem
        {
            Id = "123",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _serviceMock.Setup(s => s.GetByIdAsync("123")).ReturnsAsync(task);

        // Act
        var result = await _sut.GetById("123");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Status.Should().Be(expected);
    }

    [Theory]
    [InlineData(TaskPriority.Low, "low")]
    [InlineData(TaskPriority.Medium, "medium")]
    [InlineData(TaskPriority.High, "high")]
    public async Task GetById_MapsPriorityCorrectly(TaskPriority priority, string expected)
    {
        // Arrange
        var task = new TaskItem
        {
            Id = "123",
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _serviceMock.Setup(s => s.GetByIdAsync("123")).ReturnsAsync(task);

        // Act
        var result = await _sut.GetById("123");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        dto.Priority.Should().Be(expected);
    }

    #endregion
}
