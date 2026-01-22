using FluentAssertions;
using Moq;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using TaskBoard.Core.Services;
using Xunit;

namespace TaskBoard.Tests.Solutions;

/// <summary>
/// Soluzioni Modulo 1 e 2: Fondamenti xUnit e Unit Test Domain Layer
/// NOTA: Questi test sono le soluzioni complete degli esercizi
/// </summary>
[Trait("Category", "Solution")]
public class TaskServiceTestsSolutions
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly TaskService _sut;

    public TaskServiceTestsSolutions()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _sut = new TaskService(_repositoryMock.Object);
    }

    #region Modulo 1: Fondamenti xUnit

    /// <summary>
    /// Esercizio 1: Uso di Assert classico (xUnit nativo)
    /// </summary>
    [Fact]
    public void ParsePriority_WithLowValue_ReturnsLow()
    {
        // Arrange
        var input = "low";

        // Act
        var result = TaskService.ParsePriority(input);

        // Assert - Uso Assert classico xUnit
        Assert.Equal(TaskPriority.Low, result);
    }

    /// <summary>
    /// Esercizio 2: Theory con InlineData e FluentAssertions
    /// </summary>
    [Theory]
    [InlineData("low", TaskPriority.Low)]
    [InlineData("medium", TaskPriority.Medium)]
    [InlineData("high", TaskPriority.High)]
    [InlineData("LOW", TaskPriority.Low)]
    [InlineData("MEDIUM", TaskPriority.Medium)]
    [InlineData("invalid", TaskPriority.Medium)]
    public void ParsePriority_WithVariousInputs_ReturnsCorrectPriority(string input, TaskPriority expected)
    {
        // Act
        var result = TaskService.ParsePriority(input);

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Esercizio 3: Theory per ParseStatus
    /// </summary>
    [Theory]
    [InlineData("todo", KanbanStatus.Todo)]
    [InlineData("in-progress", KanbanStatus.InProgress)]
    [InlineData("done", KanbanStatus.Done)]
    [InlineData("TODO", KanbanStatus.Todo)]
    [InlineData("IN-PROGRESS", KanbanStatus.InProgress)]
    public void ParseStatus_WithValidValues_ReturnsCorrectStatus(string input, KanbanStatus expected)
    {
        // Act
        var result = TaskService.ParseStatus(input);

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Esercizio 4: Test valore di default
    /// </summary>
    [Fact]
    public void ParseStatus_WithInvalidValue_ReturnsDefault()
    {
        // Arrange
        var input = "invalid-status";

        // Act
        var result = TaskService.ParseStatus(input);

        // Assert
        result.Should().Be(KanbanStatus.Todo);
    }

    #endregion

    #region Modulo 2: Unit Test Domain Layer

    /// <summary>
    /// Esercizio 5: CreateAsync verifica tutti i valori
    /// </summary>
    [Fact]
    public async Task CreateAsync_WithValidData_CreatesTaskWithCorrectValues()
    {
        // Arrange
        var title = "Test Task";
        var description = "Test Description";
        var priority = "high";

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) => task);

        // Act
        var result = await _sut.CreateAsync(title, description, priority);

        // Assert
        result.Title.Should().Be(title);
        result.Description.Should().Be(description);
        result.Priority.Should().Be(TaskPriority.High);
        result.Status.Should().Be(KanbanStatus.Todo);
        result.Archived.Should().BeFalse();
    }

    /// <summary>
    /// Esercizio 6: CreateAsync imposta i timestamp
    /// </summary>
    [Fact]
    public async Task CreateAsync_SetsTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) => task);

        // Act
        var result = await _sut.CreateAsync("Task", "Desc", null);
        var after = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        result.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    /// <summary>
    /// Esercizio 7: PatchAsync aggiorna solo i campi specificati
    /// </summary>
    [Fact]
    public async Task PatchAsync_UpdatesOnlySpecifiedFields()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = "123",
            Title = "Original Title",
            Description = "Original Desc",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Medium,
            Archived = false
        };

        var updates = new Dictionary<string, object?>
        {
            ["title"] = "Updated Title"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync("123"))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
            .ReturnsAsync((string id, TaskItem task) => task);

        // Act
        var result = await _sut.PatchAsync("123", updates);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Original Desc");
        result.Status.Should().Be(KanbanStatus.Todo);
        result.Priority.Should().Be(TaskPriority.Medium);
    }

    /// <summary>
    /// Esercizio 8: PatchAsync aggiorna più campi
    /// </summary>
    [Fact]
    public async Task PatchAsync_WithMultipleFields_UpdatesAll()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = "123",
            Title = "Old Title",
            Description = "Old Desc",
            Priority = TaskPriority.Low
        };

        var updates = new Dictionary<string, object?>
        {
            ["title"] = "New Title",
            ["description"] = "New Desc",
            ["priority"] = "high"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(existingTask);
        _repositoryMock.Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
            .ReturnsAsync((string id, TaskItem task) => task);

        // Act
        var result = await _sut.PatchAsync("123", updates);

        // Assert
        result!.Title.Should().Be("New Title");
        result.Description.Should().Be("New Desc");
        result.Priority.Should().Be(TaskPriority.High);
    }

    /// <summary>
    /// Esercizio 9: UpdateAsync quando task non esiste
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenTaskNotExists_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync("nonexistent"))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.UpdateAsync("nonexistent", new TaskItem());

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<TaskItem>()), Times.Never);
    }

    /// <summary>
    /// Esercizio 10: DeleteAsync verifica chiamata repository
    /// </summary>
    [Fact]
    public async Task DeleteAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _sut.DeleteAsync("specific-id");

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync("specific-id"), Times.Once);
    }

    #endregion
}
