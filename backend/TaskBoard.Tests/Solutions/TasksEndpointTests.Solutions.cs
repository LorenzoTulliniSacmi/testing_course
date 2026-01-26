using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Moq;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Tests.Fixtures;
using TaskBoard.WebApi.DTOs;
using Xunit;

namespace TaskBoard.Tests.Solutions;

/// <summary>
/// Soluzioni Modulo 4: Integration Test HTTP
/// NOTA: Questi test sono le soluzioni complete degli esercizi
/// </summary>
[Trait("Category", "Solution")]
public class TasksEndpointTestsSolutions : IClassFixture<WebAppFixture>
{
    private readonly WebAppFixture _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public TasksEndpointTestsSolutions(WebAppFixture factory)
    {
        _factory = factory;
        _factory.ResetMock();
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Esercizio 1: GetAll restituisce Ok con JSON array
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsOkWithJsonArray()
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
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        content.Should().NotBeNull();
        content.Should().HaveCount(2);
    }

    /// <summary>
    /// Esercizio 2: GetAll restituisce proprietà in camelCase
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsCamelCaseProperties()
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

    /// <summary>
    /// Esercizio 3: Create restituisce Created con Location header
    /// </summary>
    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
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
        response.Headers.Location.Should().NotBeNull();
    }

    /// <summary>
    /// Esercizio 4: Create e GetById round-trip
    /// </summary>
    [Fact]
    public async Task CreateAndGetById_RoundTrip()
    {
        // Arrange
        var taskId = Guid.NewGuid().ToString();
        var createdTask = new TaskItem
        {
            Id = taskId,
            Title = "Round Trip Task",
            Description = "Testing round trip",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.High,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _factory.RepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(createdTask);

        _factory.SetupGetById(taskId, createdTask);

        var createDto = new CreateTaskDto
        {
            Title = "Round Trip Task",
            Description = "Testing round trip",
            Priority = "high"
        };

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto, _jsonOptions);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Act - GetById
        var getResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        var retrieved = await getResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        retrieved!.Id.Should().Be(created!.Id);
        retrieved.Title.Should().Be(created.Title);
        retrieved.Description.Should().Be(created.Description);
    }

    /// <summary>
    /// Esercizio 5: Delete restituisce NoContent
    /// </summary>
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        _factory.SetupDelete("123", true);

        // Act
        var response = await _client.DeleteAsync("/api/tasks/123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Esercizio 6: GetById con id invalido restituisce NotFound
    /// </summary>
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _factory.SetupGetById("invalid-id", null);

        // Act
        var response = await _client.GetAsync("/api/tasks/invalid-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ErrorDto>(_jsonOptions);
        content!.Error.Should().Be("Task not found");
    }

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
