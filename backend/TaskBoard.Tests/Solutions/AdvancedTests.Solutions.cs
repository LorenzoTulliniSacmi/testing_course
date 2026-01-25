using FluentAssertions;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Infrastructure.Repositories;
using TaskBoard.Tests.Fixtures;
using Xunit;

namespace TaskBoard.Tests.Solutions;

/// <summary>
/// Soluzioni Modulo 7: Test Avanzati
/// NOTA: Questi test sono le soluzioni complete degli esercizi
/// </summary>
[Collection("MongoDB")]
[Trait("Category", "Solution")]
public class AdvancedTestsSolutions : IAsyncLifetime
{
    private readonly MongoDbFixture _fixture;
    private readonly MongoTaskRepository _sut;

    public AdvancedTestsSolutions(MongoDbFixture fixture)
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
    /// Esercizio 1: Concorrenza - Last Writer Wins
    /// </summary>
    [Fact]
    public async Task ConcurrentUpdates_LastWriterWins()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Original",
            Description = "Original Desc",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _sut.CreateAsync(task);

        var update1 = new TaskItem
        {
            Title = "Update 1",
            Description = "Desc 1",
            Status = KanbanStatus.InProgress,
            Priority = TaskPriority.High,
            CreatedAt = created.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var update2 = new TaskItem
        {
            Title = "Update 2",
            Description = "Desc 2",
            Status = KanbanStatus.Done,
            Priority = TaskPriority.Low,
            CreatedAt = created.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        // Act - esegui due update in parallelo
        var task1 = _sut.UpdateAsync(created.Id, update1);
        var task2 = _sut.UpdateAsync(created.Id, update2);
        await Task.WhenAll(task1, task2);

        // Assert - verifica che uno dei due abbia vinto
        var final = await _sut.GetByIdAsync(created.Id);

        final.Should().NotBeNull();
        final!.Title.Should().BeOneOf("Update 1", "Update 2");
    }

    /// <summary>
    /// Esercizio 2: Concorrenza - Solo un Delete ha successo
    /// </summary>
    [Fact]
    public async Task ConcurrentDeletes_OnlyOneSucceeds()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "To Delete",
            Description = "Will be deleted",
            Status = KanbanStatus.Todo,
            Priority = TaskPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _sut.CreateAsync(task);

        // Act - esegui due delete in parallelo
        var delete1Task = _sut.DeleteAsync(created.Id);
        var delete2Task = _sut.DeleteAsync(created.Id);

        var results = await Task.WhenAll(delete1Task, delete2Task);

        // Assert - solo uno deve avere successo
        results.Count(r => r).Should().Be(1);
        results.Count(r => !r).Should().Be(1);

        // Verifica che il task non esista più
        var deleted = await _sut.GetByIdAsync(created.Id);
        deleted.Should().BeNull();
    }

   
}
