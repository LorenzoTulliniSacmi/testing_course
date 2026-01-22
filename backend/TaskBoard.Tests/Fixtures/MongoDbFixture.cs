using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskBoard.Infrastructure.Configuration;
using TaskBoard.Infrastructure.Models;
using Testcontainers.MongoDb;
using Xunit;

namespace TaskBoard.Tests.Fixtures;

public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container;

    public string ConnectionString { get; private set; } = string.Empty;
    public IMongoDatabase Database { get; private set; } = null!;
    public IMongoCollection<TaskDocument> TasksCollection { get; private set; } = null!;

    public MongoDbFixture()
    {
        _container = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase("kanban_test");
        TasksCollection = Database.GetCollection<TaskDocument>("tasks");
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public IOptions<MongoDbSettings> GetSettings()
    {
        return Options.Create(new MongoDbSettings
        {
            ConnectionString = ConnectionString,
            DatabaseName = "kanban_test",
            TasksCollectionName = "tasks"
        });
    }

    public async Task ClearCollectionAsync()
    {
        await TasksCollection.DeleteManyAsync(FilterDefinition<TaskDocument>.Empty);
    }

    public async Task<TaskDocument> InsertTaskAsync(TaskDocument task)
    {
        await TasksCollection.InsertOneAsync(task);
        return task;
    }

    public async Task<TaskDocument> InsertTaskAsync(
        string title,
        string description = "",
        string status = "todo",
        string priority = "medium",
        bool archived = false)
    {
        var task = new TaskDocument
        {
            Title = title,
            Description = description,
            Status = status,
            Priority = priority,
            Archived = archived,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await TasksCollection.InsertOneAsync(task);
        return task;
    }
}

[CollectionDefinition("MongoDB")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
}
