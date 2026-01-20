namespace TaskBoard.Infrastructure.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "kanban";
    public string TasksCollectionName { get; set; } = "tasks";
}
