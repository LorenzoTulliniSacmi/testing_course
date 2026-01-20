using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Core.Ports;
using TaskBoard.Infrastructure.Configuration;
using TaskBoard.Infrastructure.Repositories;

namespace TaskBoard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(
            configuration.GetSection(MongoDbSettings.SectionName));

        services.AddScoped<ITaskRepository, MongoTaskRepository>();

        return services;
    }
}
