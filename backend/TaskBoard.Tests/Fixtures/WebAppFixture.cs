using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using Xunit;

namespace TaskBoard.Tests.Fixtures;

public class WebAppFixture : WebApplicationFactory<Program>
{
    public Mock<ITaskRepository> RepositoryMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real repository
            services.RemoveAll<ITaskRepository>();

            // Add mocked repository
            services.AddScoped(_ => RepositoryMock.Object);
        });
    }

    public void SetupGetAll(params TaskItem[] tasks)
    {
        RepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<TaskQueryParams>()))
            .ReturnsAsync(tasks.ToList());
    }

    public void SetupGetById(string id, TaskItem? task)
    {
        RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(task);
    }

    public void SetupCreate()
    {
        RepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) => task);
    }

    public void SetupUpdate(string id, TaskItem? result)
    {
        RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(result);

        RepositoryMock
            .Setup(r => r.UpdateAsync(id, It.IsAny<TaskItem>()))
            .ReturnsAsync((string _, TaskItem task) => task);
    }

    public void SetupDelete(string id, bool success)
    {
        RepositoryMock
            .Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(success);
    }

    public void ResetMock()
    {
        RepositoryMock.Reset();
    }
}

public class WebAppWithMongoFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbFixture _mongoFixture;

    public MongoDbFixture MongoFixture => _mongoFixture;

    public WebAppWithMongoFixture()
    {
        _mongoFixture = new MongoDbFixture();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing MongoDB settings
            services.RemoveAll<Microsoft.Extensions.Options.IOptions<TaskBoard.Infrastructure.Configuration.MongoDbSettings>>();
            services.RemoveAll<Microsoft.Extensions.Options.IOptionsSnapshot<TaskBoard.Infrastructure.Configuration.MongoDbSettings>>();
            services.RemoveAll<Microsoft.Extensions.Options.IOptionsMonitor<TaskBoard.Infrastructure.Configuration.MongoDbSettings>>();

            // Add test MongoDB settings
            services.Configure<TaskBoard.Infrastructure.Configuration.MongoDbSettings>(options =>
            {
                options.ConnectionString = _mongoFixture.ConnectionString;
                options.DatabaseName = "kanban_test";
                options.TasksCollectionName = "tasks";
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoFixture.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}
