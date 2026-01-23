using System.Text.Json.Serialization;
using TaskBoard.Infrastructure.Extensions;
using TaskBoard.WebApi.Extensions;
using TaskBoard.WebApi.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Kanban Board API",
        Version = "v1",
        Description = "REST API for the Kanban Board application"
    });
});

// Add CORS
builder.Services.AddCorsPolicy("CorsPolicy", builder.Configuration);

// Add Infrastructure (MongoDB)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application Services
builder.Services.AddApplicationServices();

// Configure Mapster
MappingConfig.Configure();

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kanban Board API v1");
    options.RoutePrefix = "api-docs";
});

app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    timestamp = DateTime.UtcNow.ToString("o")
}));

app.Run();

// Required for WebApplicationFactory access in integration tests
public partial class Program { }
