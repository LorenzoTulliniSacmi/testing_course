using FluentAssertions;
using Moq;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using TaskBoard.Core.Services;

namespace TaskBoard.Tests.Unit.Core;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _sut = new TaskService(_repositoryMock.Object);
    }

    #region CreateAsync Tests

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
        result.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithNullPriority_DefaultsToMedium()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) => task);

        // Act
        var result = await _sut.CreateAsync("Task", "Desc", null);

        // Assert
        result.Priority.Should().Be(TaskPriority.Medium);
    }

    [Theory]
    [InlineData("low", TaskPriority.Low)]
    [InlineData("medium", TaskPriority.Medium)]
    [InlineData("high", TaskPriority.High)]
    [InlineData("LOW", TaskPriority.Low)]
    [InlineData("HIGH", TaskPriority.High)]
    [InlineData("invalid", TaskPriority.Medium)]
    public async Task CreateAsync_ParsesPriorityCorrectly(string input, TaskPriority expected)
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) => task);

        // Act
        var result = await _sut.CreateAsync("Task", "Desc", input);

        // Assert
        result.Priority.Should().Be(expected);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtAndUpdatedAt()
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

    [Fact]
    public async Task CreateAsync_CallsRepositoryOnce()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) => task);

        // Act
        await _sut.CreateAsync("Task", "Desc", null);

        // Assert
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsTasksFromRepository()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new() { Id = "1", Title = "Task 1" },
            new() { Id = "2", Title = "Task 2" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<TaskQueryParams>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetAllAsync(new TaskQueryParams());

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
    }

    [Fact]
    public async Task GetAllAsync_PassesQueryParamsToRepository()
    {
        // Arrange
        var query = new TaskQueryParams(
            Status: KanbanStatus.Todo,
            Priority: TaskPriority.High,
            Archived: "false"
        );

        _repositoryMock
            .Setup(r => r.GetAllAsync(query))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        await _sut.GetAllAsync(query);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(query), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsTask()
    {
        // Arrange
        var task = new TaskItem { Id = "123", Title = "Test Task" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync("123"))
            .ReturnsAsync(task);

        // Act
        var result = await _sut.GetByIdAsync("123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("123");
        result.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskNotExists_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync("nonexistent"))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.GetByIdAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenTaskExists_UpdatesAllFields()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = "123",
            Title = "Old Title",
            Description = "Old Desc",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Low,
            Archived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updates = new TaskItem
        {
            Title = "New Title",
            Description = "New Desc",
            Status = KanbanStatus.Done,
            Priority = TaskPriority.High,
            Archived = true
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync("123"))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
            .ReturnsAsync((string id, TaskItem task) => task);

        // Act
        var result = await _sut.UpdateAsync("123", updates);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("New Title");
        result.Description.Should().Be("New Desc");
        result.Status.Should().Be(KanbanStatus.Done);
        result.Priority.Should().Be(TaskPriority.High);
        result.Archived.Should().BeTrue();
    }

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

    [Fact]
    public async Task UpdateAsync_UpdatesUpdatedAtTimestamp()
    {
        // Arrange
        var oldTimestamp = DateTime.UtcNow.AddDays(-1);
        var existingTask = new TaskItem
        {
            Id = "123",
            UpdatedAt = oldTimestamp
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync("123"))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
            .ReturnsAsync((string id, TaskItem task) => task);

        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.UpdateAsync("123", new TaskItem());
        var after = DateTime.UtcNow;

        // Assert
        result!.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    #endregion

    #region PatchAsync Tests

    [Fact]
    public async Task PatchAsync_WhenTaskExists_UpdatesOnlySpecifiedFields()
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

    [Fact]
    public async Task PatchAsync_WhenTaskNotExists_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync("nonexistent"))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.PatchAsync("nonexistent", new Dictionary<string, object?>());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task PatchAsync_UpdatesStatus()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = "123",
            Status = KanbanStatus.Todo
        };

        var updates = new Dictionary<string, object?>
        {
            ["status"] = "done"
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
        result!.Status.Should().Be(KanbanStatus.Done);
    }

    [Theory]
    [InlineData("todo", KanbanStatus.Todo)]
    [InlineData("in-progress", KanbanStatus.InProgress)]
    [InlineData("done", KanbanStatus.Done)]
    public async Task PatchAsync_ParsesStatusCorrectly(string input, KanbanStatus expected)
    {
        // Arrange
        var existingTask = new TaskItem { Id = "123" };
        var updates = new Dictionary<string, object?> { ["status"] = input };

        _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(existingTask);
        _repositoryMock.Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
            .ReturnsAsync((string id, TaskItem task) => task);

        // Act
        var result = await _sut.PatchAsync("123", updates);

        // Assert
        result!.Status.Should().Be(expected);
    }

    [Fact]
    public async Task PatchAsync_UpdatesArchived()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = "123",
            Archived = false
        };

        var updates = new Dictionary<string, object?>
        {
            ["archived"] = true
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
        result!.Archived.Should().BeTrue();
    }

    [Fact]
    public async Task PatchAsync_UpdatesMultipleFields()
    {
        // Arrange
        var existingTask = new TaskItem
        {
            Id = "123",
            Title = "Old",
            Description = "Old Desc",
            Priority = TaskPriority.Low
        };

        var updates = new Dictionary<string, object?>
        {
            ["title"] = "New Title",
            ["description"] = "New Desc",
            ["priority"] = "high"
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
        result!.Title.Should().Be("New Title");
        result.Description.Should().Be("New Desc");
        result.Priority.Should().Be(TaskPriority.High);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenTaskExists_ReturnsTrue()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.DeleteAsync("123"))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteAsync("123");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskNotExists_ReturnsFalse()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.DeleteAsync("nonexistent"))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.DeleteAsync("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

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
