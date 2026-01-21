using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Tests.Fixtures;
using TaskBoard.WebApi.DTOs;

namespace TaskBoard.Tests.Integration.Api;

public class TasksEndpointTests : IClassFixture<WebAppFixture>
{
    private readonly WebAppFixture _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public TasksEndpointTests(WebAppFixture factory)
    {
        _factory = factory;
        _factory.ResetMock();
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region GET /api/tasks Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithTasks()
    {
        // Arrange
        var tasks = new[]
        {
            CreateTaskItem("1", "Task 1", KanbanStatus.Todo, TaskPriority.Medium),
            CreateTaskItem("2", "Task 2", KanbanStatus.Done, TaskPriority.High)
        };

        _factory.SetupGetAll(tasks);

        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        content.Should().HaveCount(2);
        content![0].Title.Should().Be("Task 1");
        content[1].Title.Should().Be("Task 2");
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyArray_WhenNoTasks()
    {
        // Arrange
        _factory.SetupGetAll();

        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsCamelCaseJson()
    {
        // Arrange
        var task = CreateTaskItem("1", "Task 1", KanbanStatus.InProgress, TaskPriority.High);
        _factory.SetupGetAll(task);

        // Act
        var response = await _client.GetAsync("/api/tasks");
        var jsonString = await response.Content.ReadAsStringAsync();

        // Assert
        jsonString.Should().Contain("\"createdAt\"");
        jsonString.Should().Contain("\"updatedAt\"");
        jsonString.Should().NotContain("\"CreatedAt\"");
        jsonString.Should().NotContain("\"UpdatedAt\"");
    }

    #endregion

    #region GET /api/tasks/{id} Tests

    [Fact]
    public async Task GetById_WhenTaskExists_ReturnsOk()
    {
        // Arrange
        var task = CreateTaskItem("abc123", "Test Task", KanbanStatus.Todo, TaskPriority.Medium);
        _factory.SetupGetById("abc123", task);

        // Act
        var response = await _client.GetAsync("/api/tasks/abc123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        content!.Id.Should().Be("abc123");
        content.Title.Should().Be("Test Task");
        content.Status.Should().Be("todo");
        content.Priority.Should().Be("medium");
    }

    [Fact]
    public async Task GetById_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _factory.SetupGetById("nonexistent", null);

        // Act
        var response = await _client.GetAsync("/api/tasks/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ErrorDto>(_jsonOptions);
        content!.Error.Should().Be("Task not found");
    }

    #endregion

    #region POST /api/tasks Tests

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        _factory.SetupCreate();

        var createDto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "New Description",
            Priority = "high"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        content!.Title.Should().Be("New Task");
        content.Description.Should().Be("New Description");
        content.Priority.Should().Be("high");
        content.Status.Should().Be("todo");

        response.Headers.Location.Should().NotBeNull();
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
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<ErrorDto>(_jsonOptions);
        content!.Error.Should().Be("Title is required");
    }

    [Fact]
    public async Task Create_WithDefaultPriority_SetsMedium()
    {
        // Arrange
        _factory.SetupCreate();

        var createDto = new CreateTaskDto
        {
            Title = "Task without priority",
            Description = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        content!.Priority.Should().Be("medium");
    }

    #endregion

    #region PUT /api/tasks/{id} Tests

    [Fact]
    public async Task Update_WithValidData_ReturnsOk()
    {
        // Arrange
        var existingTask = CreateTaskItem("123", "Old Title", KanbanStatus.Todo, TaskPriority.Low);
        _factory.SetupUpdate("123", existingTask);

        var updateDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Desc",
            Status = "done",
            Priority = "high",
            Archived = false
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/tasks/123", updateDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        content!.Title.Should().Be("Updated Title");
        content.Status.Should().Be("done");
        content.Priority.Should().Be("high");
    }

    [Fact]
    public async Task Update_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _factory.SetupUpdate("nonexistent", null);

        var updateDto = new UpdateTaskDto
        {
            Title = "Title",
            Description = "Desc",
            Status = "todo",
            Priority = "medium"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/tasks/nonexistent", updateDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithMissingFields_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateTaskDto
        {
            Title = "Title",
            Description = "",
            Status = "todo",
            Priority = "medium"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/tasks/123", updateDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PATCH /api/tasks/{id} Tests

    [Fact]
    public async Task Patch_WithValidData_ReturnsOk()
    {
        // Arrange
        var existingTask = CreateTaskItem("123", "Original Title", KanbanStatus.Todo, TaskPriority.Medium);
        _factory.SetupUpdate("123", existingTask);

        var patchDto = new PatchTaskDto
        {
            Title = "Patched Title"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/tasks/123", patchDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        content!.Title.Should().Be("Patched Title");
    }

    [Fact]
    public async Task Patch_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _factory.SetupUpdate("nonexistent", null);

        var patchDto = new PatchTaskDto { Title = "New Title" };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/tasks/nonexistent", patchDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Patch_WithArchivedFlag_ReturnsOk()
    {
        // Arrange
        var existingTask = CreateTaskItem("123", "Title", KanbanStatus.Done, TaskPriority.Low);
        _factory.SetupUpdate("123", existingTask);

        var patchDto = new PatchTaskDto { Archived = true };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/tasks/123", patchDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region DELETE /api/tasks/{id} Tests

    [Fact]
    public async Task Delete_WhenTaskExists_ReturnsNoContent()
    {
        // Arrange
        _factory.SetupDelete("123", true);

        // Act
        var response = await _client.DeleteAsync("/api/tasks/123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _factory.SetupDelete("nonexistent", false);

        // Act
        var response = await _client.DeleteAsync("/api/tasks/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Health Endpoint Tests

    [Fact]
    public async Task Health_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("ok");
        content.Should().Contain("timestamp");
    }

    #endregion

    #region Helper Methods

    private static TaskItem CreateTaskItem(
        string id,
        string title,
        KanbanStatus status,
        TaskPriority priority)
    {
        return new TaskItem
        {
            Id = id,
            Title = title,
            Description = "Test Description",
            Status = status,
            Priority = priority,
            Archived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion
}
