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

namespace TaskBoard.Tests.Unit.WebApi;

/// <summary>
/// Esercizi Modulo 3: Unit Test Controller
///
/// ISTRUZIONI:
/// - Questi test verificano il comportamento del TasksController
/// - Focus su ActionResult, status code e DTO di risposta
/// - Usa FluentAssertions per tutte le asserzioni
/// - Esegui: dotnet test --filter "FullyQualifiedName~TasksControllerTests"
/// </summary>
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

    /// <summary>
    /// Modulo 3: Unit Test Controller
    /// Esercizio 1: GetById restituisce NotFound quando il task non esiste
    ///
    /// Obiettivo: Verificare che GetById restituisca 404 NotFound con ErrorDto
    /// </summary>
    [Fact]
    public async Task GetById_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        // TODO: Configura il service mock per restituire null quando viene chiamato GetByIdAsync
        // HINT: _serviceMock
        //     .Setup(s => s.GetByIdAsync("nonexistent"))
        //     .ReturnsAsync((TaskItem?)null);

        // Act
        // TODO: Chiama GetById del controller
        // HINT: var result = await _sut.GetById("nonexistent");

        // Assert
        // TODO: Verifica che:
        // - result.Result sia di tipo NotFoundObjectResult
        // - StatusCode sia 404
        // - Value contenga un ErrorDto con Error = "Task not found"
        // HINT: var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        //       notFoundResult.StatusCode.Should().Be(404);
        //       var error = notFoundResult.Value.Should().BeOfType<ErrorDto>().Subject;
        //       error.Error.Should().Be("Task not found");

        throw new NotImplementedException("Esercizio 1: Implementa il test NotFound");
    }

    /// <summary>
    /// Modulo 3: Unit Test Controller
    /// Esercizio 2: GetById restituisce Ok con TaskDto quando il task esiste
    ///
    /// Obiettivo: Verificare che GetById restituisca 200 OK con il TaskDto corretto
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

        // TODO: Configura il service mock per restituire il task
        // HINT: _serviceMock
        //     .Setup(s => s.GetByIdAsync("123"))
        //     .ReturnsAsync(task);

        // Act
        // TODO: Chiama GetById del controller
        // HINT: var result = await _sut.GetById("123");

        // Assert
        // TODO: Verifica che:
        // - result.Result sia di tipo OkObjectResult
        // - StatusCode sia 200
        // - Value sia un TaskDto con i dati corretti
        // HINT: var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        //       okResult.StatusCode.Should().Be(200);
        //       var dto = okResult.Value.Should().BeOfType<TaskDto>().Subject;
        //       dto.Id.Should().Be("123");
        //       dto.Title.Should().Be("Test Task");

        throw new NotImplementedException("Esercizio 2: Implementa il test Ok con TaskDto");
    }

    /// <summary>
    /// Modulo 3: Unit Test Controller
    /// Esercizio 3: Create con titolo vuoto restituisce BadRequest
    ///
    /// Obiettivo: Verificare la validazione del titolo vuoto
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
        // TODO: Chiama Create del controller con il DTO
        // HINT: var result = await _sut.Create(createDto);

        // Assert
        // TODO: Verifica che:
        // - result.Result sia di tipo BadRequestObjectResult
        // - StatusCode sia 400
        // - ErrorDto.Error contenga "Title is required"
        // HINT: var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        //       badRequestResult.StatusCode.Should().Be(400);
        //       var error = badRequestResult.Value.Should().BeOfType<ErrorDto>().Subject;
        //       error.Error.Should().Be("Title is required");

        throw new NotImplementedException("Esercizio 3: Implementa il test BadRequest per titolo vuoto");
    }

    /// <summary>
    /// Modulo 3: Unit Test Controller
    /// Esercizio 4: Create con dati validi restituisce CreatedAtAction
    ///
    /// Obiettivo: Verificare che Create restituisca 201 Created con location header
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

        // TODO: Configura il service mock per restituire il task creato
        // HINT: _serviceMock
        //     .Setup(s => s.CreateAsync("New Task", "New Description", "high"))
        //     .ReturnsAsync(createdTask);

        // Act
        // TODO: Chiama Create del controller
        // HINT: var result = await _sut.Create(createDto);

        // Assert
        // TODO: Verifica che:
        // - result.Result sia di tipo CreatedAtActionResult
        // - StatusCode sia 201
        // - ActionName punti a GetById
        // - Value contenga il TaskDto con l'ID corretto
        // HINT: var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        //       createdResult.StatusCode.Should().Be(201);
        //       createdResult.ActionName.Should().Be(nameof(TasksController.GetById));
        //       var dto = createdResult.Value.Should().BeOfType<TaskDto>().Subject;
        //       dto.Id.Should().Be("new-id");

        throw new NotImplementedException("Esercizio 4: Implementa il test CreatedAtAction");
    }

    /// <summary>
    /// Modulo 3: Unit Test Controller
    /// Esercizio 5: Delete quando il task esiste restituisce NoContent
    ///
    /// Obiettivo: Verificare che Delete restituisca 204 NoContent
    /// </summary>
    [Fact]
    public async Task Delete_WhenTaskExists_ReturnsNoContent()
    {
        // Arrange
        // TODO: Configura il service mock per restituire true (delete riuscito)
        // HINT: _serviceMock
        //     .Setup(s => s.DeleteAsync("123"))
        //     .ReturnsAsync(true);

        // Act
        // TODO: Chiama Delete del controller
        // HINT: var result = await _sut.Delete("123");

        // Assert
        // TODO: Verifica che result sia di tipo NoContentResult (204)
        // HINT: result.Should().BeOfType<NoContentResult>();
        //       (result as NoContentResult)!.StatusCode.Should().Be(204);

        throw new NotImplementedException("Esercizio 5: Implementa il test NoContent per Delete");
    }

    /// <summary>
    /// Modulo 3: Unit Test Controller
    /// Esercizio 6: Delete quando il task non esiste restituisce NotFound
    ///
    /// Obiettivo: Verificare che Delete restituisca 404 NotFound con ErrorDto
    /// </summary>
    [Fact]
    public async Task Delete_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        // TODO: Configura il service mock per restituire false (task non trovato)
        // HINT: _serviceMock
        //     .Setup(s => s.DeleteAsync("nonexistent"))
        //     .ReturnsAsync(false);

        // Act
        // TODO: Chiama Delete del controller
        // HINT: var result = await _sut.Delete("nonexistent");

        // Assert
        // TODO: Verifica che:
        // - result sia di tipo NotFoundObjectResult
        // - StatusCode sia 404
        // - ErrorDto.Error sia "Task not found"
        // HINT: var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        //       notFoundResult.StatusCode.Should().Be(404);
        //       var error = notFoundResult.Value.Should().BeOfType<ErrorDto>().Subject;
        //       error.Error.Should().Be("Task not found");

        throw new NotImplementedException("Esercizio 6: Implementa il test NotFound per Delete");
    }
}
