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

namespace TaskBoard.Tests.Integration.Api;

/// <summary>
/// Esercizi Modulo 4: Integration Test HTTP
///
/// ISTRUZIONI:
/// - Questi test verificano l'API HTTP end-to-end usando WebApplicationFactory
/// - Focus su status code HTTP, JSON serialization e header
/// - La fixture WebAppFixture usa un repository mockato
/// - Esegui: dotnet test --filter "FullyQualifiedName~TasksEndpointTests"
/// </summary>
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

    /// <summary>
    /// Modulo 4: Integration Test HTTP
    /// Esercizio 1: GET /api/tasks restituisce 200 OK con JSON array
    ///
    /// Obiettivo: Verificare che l'endpoint restituisca correttamente una lista di task
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

        // TODO: Configura il mock per restituire i task
        // HINT: _factory.SetupGetAll(tasks);

        // Act
        // TODO: Esegui una GET request a /api/tasks
        // HINT: var response = await _client.GetAsync("/api/tasks");

        // Assert
        // TODO: Verifica che:
        // - StatusCode sia 200 OK
        // - ContentType sia application/json
        // - Il body contenga 2 elementi
        // HINT: response.StatusCode.Should().Be(HttpStatusCode.OK);
        //       response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        //       var content = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        //       content.Should().HaveCount(2);

        throw new NotImplementedException("Esercizio 1: Implementa il test GetAll");
    }

    /// <summary>
    /// Modulo 4: Integration Test HTTP
    /// Esercizio 2: GET /api/tasks restituisce proprietà in camelCase
    ///
    /// Obiettivo: Verificare che la serializzazione JSON usi camelCase (non PascalCase)
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsCamelCaseProperties()
    {
        // Arrange
        var task = CreateTaskItem("1", "Task 1", KanbanStatus.InProgress, TaskPriority.High);

        // TODO: Configura il mock
        // HINT: _factory.SetupGetAll(task);

        // Act
        // TODO: Esegui la GET e leggi il JSON come stringa raw
        // HINT: var response = await _client.GetAsync("/api/tasks");
        //       var jsonString = await response.Content.ReadAsStringAsync();

        // Assert
        // TODO: Verifica che il JSON contenga "createdAt" (camelCase) e NON "CreatedAt" (PascalCase)
        // HINT: jsonString.Should().Contain("\"createdAt\"");
        //       jsonString.Should().Contain("\"updatedAt\"");
        //       jsonString.Should().NotContain("\"CreatedAt\"");
        //       jsonString.Should().NotContain("\"UpdatedAt\"");

        throw new NotImplementedException("Esercizio 2: Implementa il test camelCase");
    }

    /// <summary>
    /// Modulo 4: Integration Test HTTP
    /// Esercizio 3: POST /api/tasks restituisce Created con Location header
    ///
    /// Obiettivo: Verificare che la creazione restituisca 201 e includa il Location header
    /// </summary>
    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        // Arrange
        // TODO: Configura il mock per la creazione
        // HINT: _factory.SetupCreate();

        var createDto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "New Description",
            Priority = "high"
        };

        // Act
        // TODO: Esegui una POST request con il body JSON
        // HINT: var response = await _client.PostAsJsonAsync("/api/tasks", createDto, _jsonOptions);

        // Assert
        // TODO: Verifica che:
        // - StatusCode sia 201 Created
        // - Location header sia presente e contenga /api/tasks/
        // HINT: response.StatusCode.Should().Be(HttpStatusCode.Created);
        //       response.Headers.Location.Should().NotBeNull();
        //       response.Headers.Location!.ToString().Should().Contain("/api/tasks/");

        throw new NotImplementedException("Esercizio 3: Implementa il test Create con Location header");
    }

    /// <summary>
    /// Modulo 4: Integration Test HTTP
    /// Esercizio 4: Create e GetById round-trip
    ///
    /// Obiettivo: Verificare che un task creato possa essere recuperato via GetById
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

        // TODO: Configura i mock per Create e GetById
        // HINT: _factory.RepositoryMock
        //     .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
        //     .ReturnsAsync(createdTask);
        // _factory.SetupGetById(taskId, createdTask);

        var createDto = new CreateTaskDto
        {
            Title = "Round Trip Task",
            Description = "Testing round trip",
            Priority = "high"
        };

        // Act
        // TODO: 1. Crea il task via POST
        // HINT: var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto, _jsonOptions);
        //       var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // TODO: 2. Recupera il task via GET
        // HINT: var getResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        //       var retrieved = await getResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Assert
        // TODO: Verifica che:
        // - Entrambe le response siano success (201 e 200)
        // - I dati del task recuperato corrispondano a quelli creati
        // HINT: createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        //       getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        //       retrieved!.Id.Should().Be(created!.Id);
        //       retrieved.Title.Should().Be(created.Title);

        throw new NotImplementedException("Esercizio 4: Implementa il test round-trip");
    }

    /// <summary>
    /// Modulo 4: Integration Test HTTP
    /// Esercizio 5: DELETE /api/tasks/{id} restituisce NoContent
    ///
    /// Obiettivo: Verificare che la cancellazione restituisca 204 NoContent
    /// </summary>
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        // TODO: Configura il mock per il delete
        // HINT: _factory.SetupDelete("123", true);

        // Act
        // TODO: Esegui una DELETE request
        // HINT: var response = await _client.DeleteAsync("/api/tasks/123");

        // Assert
        // TODO: Verifica che StatusCode sia 204 NoContent
        // HINT: response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        throw new NotImplementedException("Esercizio 5: Implementa il test Delete");
    }

    /// <summary>
    /// Modulo 4: Integration Test HTTP
    /// Esercizio 6: GET /api/tasks/{id} con ID invalido restituisce NotFound
    ///
    /// Obiettivo: Verificare che l'endpoint restituisca 404 con ErrorDto
    /// </summary>
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        // TODO: Configura il mock per restituire null (task non trovato)
        // HINT: _factory.SetupGetById("invalid-id", null);

        // Act
        // TODO: Esegui una GET request con un ID che non esiste
        // HINT: var response = await _client.GetAsync("/api/tasks/invalid-id");

        // Assert
        // TODO: Verifica che:
        // - StatusCode sia 404 NotFound
        // - Il body contenga un ErrorDto con messaggio appropriato
        // HINT: response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //       var content = await response.Content.ReadFromJsonAsync<ErrorDto>(_jsonOptions);
        //       content!.Error.Should().Be("Task not found");

        throw new NotImplementedException("Esercizio 6: Implementa il test NotFound");
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
