using FluentAssertions;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using TaskBoard.Infrastructure.Repositories;
using TaskBoard.Tests.Fixtures;
using Xunit;

namespace TaskBoard.Tests.Solutions;

/// <summary>
/// Soluzioni Modulo 5: Integration Test MongoDB
/// NOTA: Questi test sono le soluzioni complete degli esercizi
/// </summary>
[Collection("MongoDB")]
[Trait("Category", "Solution")]
public class MongoTaskRepositoryTestsSolutions : IAsyncLifetime
{
    private readonly MongoDbFixture _fixture;
    private readonly MongoTaskRepository _sut;

    public MongoTaskRepositoryTestsSolutions(MongoDbFixture fixture)
    {
        _fixture = fixture;
        _sut = new MongoTaskRepository(_fixture.GetSettings());
    }

    public async Task InitializeAsync()
    {
        await _fixture.ClearCollectionAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Esercizio 1: Filtro per status
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ReturnsMatchingTasks()
    {
        // Arrange
        await _fixture.InsertTaskAsync("Todo Task 1", status: "todo");
        await _fixture.InsertTaskAsync("Todo Task 2", status: "todo");
        await _fixture.InsertTaskAsync("Done Task", status: "done");

        var query = new TaskQueryParams(Status: KanbanStatus.Todo);

        // Act
        var result = await _sut.GetAllAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Status.Should().Be(KanbanStatus.Todo));
    }

    /// <summary>
    /// Esercizio 2: Filtro per priority
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WithPriorityFilter_ReturnsMatchingTasks()
    {
        // Arrange
        await _fixture.InsertTaskAsync("High Priority", priority: "high");
        await _fixture.InsertTaskAsync("Low Priority", priority: "low");
        await _fixture.InsertTaskAsync("Another High", priority: "high");

        var query = new TaskQueryParams(Priority: TaskPriority.High);

        // Act
        var result = await _sut.GetAllAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Priority.Should().Be(TaskPriority.High));
    }

    /// <summary>
    /// Esercizio 3: Filtro per archived (Theory)
    /// </summary>
    [Theory]
    [InlineData("true", 2)]
    [InlineData("false", 3)]
    [InlineData("all", 5)]
    public async Task GetAllAsync_WithArchivedFilter_ReturnsCorrectTasks(string archivedFilter, int expectedCount)
    {
        // Arrange
        await _fixture.InsertTaskAsync("Active 1", archived: false);
        await _fixture.InsertTaskAsync("Active 2", archived: false);
        await _fixture.InsertTaskAsync("Active 3", archived: false);
        await _fixture.InsertTaskAsync("Archived 1", archived: true);
        await _fixture.InsertTaskAsync("Archived 2", archived: true);

        var query = new TaskQueryParams(Archived: archivedFilter);

        // Act
        var result = await _sut.GetAllAsync(query);

        // Assert
        result.Should().HaveCount(expectedCount);
    }

    /// <summary>
    /// Esercizio 4: Search case-insensitive
    /// </summary>
    [Fact]
    public async Task GetAllAsync_WithSearch_FindsCaseInsensitive()
    {
        // Arrange
        await _fixture.InsertTaskAsync("IMPORTANT Task");
        await _fixture.InsertTaskAsync("Important Note");
        await _fixture.InsertTaskAsync("Something else");

        var query = new TaskQueryParams(Search: "important", Archived: "all");

        // Act
        var result = await _sut.GetAllAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t =>
            t.Title.Should().ContainEquivalentOf("important"));
    }

    /// <summary>
    /// Esercizio 5: Ordinamento per CreatedAt (Theory)
    /// </summary>
    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    public async Task GetAllAsync_OrderByCreatedAt_ReturnsSorted(string order)
    {
        // Arrange
        await _fixture.InsertTaskAsync("First");
        await Task.Delay(10);
        await _fixture.InsertTaskAsync("Second");
        await Task.Delay(10);
        await _fixture.InsertTaskAsync("Third");

        var query = new TaskQueryParams(OrderBy: "createdAt", Order: order, Archived: "all");

        // Act
        var result = (await _sut.GetAllAsync(query)).ToList();

        // Assert
        result.Should().HaveCount(3);

        if (order == "asc")
        {
            result[0].Title.Should().Be("First");
            result[2].Title.Should().Be("Third");
        }
        else
        {
            result[0].Title.Should().Be("Third");
            result[2].Title.Should().Be("First");
        }
    }

    /// <summary>
    /// Esercizio 6: Ordinamento per Priority (Theory)
    /// </summary>
    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    public async Task GetAllAsync_OrderByPriority_ReturnsSorted(string order)
    {
        // Arrange
        await _fixture.InsertTaskAsync("Medium Task", priority: "medium");
        await _fixture.InsertTaskAsync("High Task", priority: "high");
        await _fixture.InsertTaskAsync("Low Task", priority: "low");

        var query = new TaskQueryParams(OrderBy: "priority", Order: order, Archived: "all");

        // Act
        var result = (await _sut.GetAllAsync(query)).ToList();

        // Assert
        result.Should().HaveCount(3);

        if (order == "asc")
        {
            result[0].Priority.Should().Be(TaskPriority.Low);
            result[2].Priority.Should().Be(TaskPriority.High);
        }
        else
        {
            result[0].Priority.Should().Be(TaskPriority.High);
            result[2].Priority.Should().Be(TaskPriority.Low);
        }
    }

    /// <summary>
    /// Esercizio 7: CreateAsync genera ObjectId valido
    /// </summary>
    [Fact]
    public async Task CreateAsync_GeneratesValidObjectId()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "New Task",
            Description = "Description",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _sut.CreateAsync(task);

        // Assert
        result.Id.Should().NotBeNullOrEmpty();
        result.Id.Should().HaveLength(24);
        result.Id.Should().MatchRegex("^[a-f0-9]{24}$");
    }

    /// <summary>
    /// Esercizio 8: DeleteAsync con ObjectId invalido
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithInvalidObjectId_ReturnsFalse()
    {
        // Arrange
        var invalidId = "not-a-valid-objectid";

        // Act
        var result = await _sut.DeleteAsync(invalidId);

        // Assert
        result.Should().BeFalse();
    }
}
