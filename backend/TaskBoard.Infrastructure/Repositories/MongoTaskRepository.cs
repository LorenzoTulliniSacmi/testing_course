using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TaskBoard.Core.Models;
using TaskBoard.Core.Models.Enums;
using TaskBoard.Core.Ports;
using TaskBoard.Infrastructure.Configuration;
using TaskBoard.Infrastructure.Models;

namespace TaskBoard.Infrastructure.Repositories;

public class MongoTaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskDocument> _collection;

    public MongoTaskRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<TaskDocument>(settings.Value.TasksCollectionName);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(TaskQueryParams query)
    {
        var filterBuilder = Builders<TaskDocument>.Filter;
        var filters = new List<FilterDefinition<TaskDocument>>();

        // Filter by archived (default: show non-archived)
        if (query.Archived != "all")
        {
            var showArchived = query.Archived == "true";
            filters.Add(filterBuilder.Eq(t => t.Archived, showArchived));
        }

        // Filter by status
        if (query.Status.HasValue)
        {
            var statusString = StatusToString(query.Status.Value);
            filters.Add(filterBuilder.Eq(t => t.Status, statusString));
        }

        // Filter by priority
        if (query.Priority.HasValue)
        {
            var priorityString = PriorityToString(query.Priority.Value);
            filters.Add(filterBuilder.Eq(t => t.Priority, priorityString));
        }

        // Search by title (case-insensitive)
        if (!string.IsNullOrEmpty(query.Search))
        {
            filters.Add(filterBuilder.Regex(t => t.Title,
                new BsonRegularExpression(query.Search, "i")));
        }

        var filter = filters.Count > 0
            ? filterBuilder.And(filters)
            : filterBuilder.Empty;

        var findFluent = _collection.Find(filter);

        // Sorting
        if (!string.IsNullOrEmpty(query.OrderBy))
        {
            var isDescending = query.Order == "desc";

            if (query.OrderBy == "priority")
            {
                // Sort in memory for priority (custom order)
                var docs = await findFluent.ToListAsync();
                var sorted = isDescending
                    ? docs.OrderByDescending(d => PriorityOrder(d.Priority))
                    : docs.OrderBy(d => PriorityOrder(d.Priority));
                return sorted.Select(MapToTaskItem);
            }

            var sortBuilder = Builders<TaskDocument>.Sort;
            SortDefinition<TaskDocument> sort = query.OrderBy switch
            {
                "createdAt" => isDescending
                    ? sortBuilder.Descending(t => t.CreatedAt)
                    : sortBuilder.Ascending(t => t.CreatedAt),
                "updatedAt" => isDescending
                    ? sortBuilder.Descending(t => t.UpdatedAt)
                    : sortBuilder.Ascending(t => t.UpdatedAt),
                "title" => isDescending
                    ? sortBuilder.Descending(t => t.Title)
                    : sortBuilder.Ascending(t => t.Title),
                _ => sortBuilder.Ascending(t => t.CreatedAt)
            };

            findFluent = findFluent.Sort(sort);
        }

        var documents = await findFluent.ToListAsync();
        return documents.Select(MapToTaskItem);
    }

    public async Task<TaskItem?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        var doc = await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : MapToTaskItem(doc);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        var document = new TaskDocument
        {
            Title = task.Title,
            Description = task.Description,
            Status = StatusToString(task.Status),
            Priority = PriorityToString(task.Priority),
            Archived = task.Archived,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        await _collection.InsertOneAsync(document);
        return MapToTaskItem(document);
    }

    public async Task<TaskItem?> UpdateAsync(string id, TaskItem task)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        var update = Builders<TaskDocument>.Update
            .Set(t => t.Title, task.Title)
            .Set(t => t.Description, task.Description)
            .Set(t => t.Status, StatusToString(task.Status))
            .Set(t => t.Priority, PriorityToString(task.Priority))
            .Set(t => t.Archived, task.Archived)
            .Set(t => t.UpdatedAt, task.UpdatedAt);

        var options = new FindOneAndUpdateOptions<TaskDocument, TaskDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updated = await _collection.FindOneAndUpdateAsync<TaskDocument, TaskDocument>(
            t => t.Id == id, update, options);

        return updated == null ? null : MapToTaskItem(updated);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await _collection.DeleteOneAsync(t => t.Id == id);
        return result.DeletedCount > 0;
    }

    private static TaskItem MapToTaskItem(TaskDocument doc) => new()
    {
        Id = doc.Id,
        Title = doc.Title,
        Description = doc.Description,
        Status = ParseStatus(doc.Status),
        Priority = ParsePriority(doc.Priority),
        Archived = doc.Archived,
        CreatedAt = doc.CreatedAt,
        UpdatedAt = doc.UpdatedAt
    };

    private static string StatusToString(KanbanStatus status) => status switch
    {
        KanbanStatus.InProgress => "in-progress",
        KanbanStatus.Done => "done",
        _ => "todo"
    };

    private static string PriorityToString(TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "low",
        TaskPriority.High => "high",
        _ => "medium"
    };

    private static KanbanStatus ParseStatus(string status) => status switch
    {
        "in-progress" => KanbanStatus.InProgress,
        "done" => KanbanStatus.Done,
        _ => KanbanStatus.Todo
    };

    private static TaskPriority ParsePriority(string priority) => priority switch
    {
        "low" => TaskPriority.Low,
        "high" => TaskPriority.High,
        _ => TaskPriority.Medium
    };

    private static int PriorityOrder(string priority) => priority switch
    {
        "high" => 3,
        "medium" => 2,
        "low" => 1,
        _ => 2
    };
}
