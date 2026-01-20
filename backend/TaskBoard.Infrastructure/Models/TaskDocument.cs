using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskBoard.Infrastructure.Models;

public class TaskDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "todo";

    [BsonElement("priority")]
    public string Priority { get; set; } = "medium";

    [BsonElement("archived")]
    public bool Archived { get; set; } = false;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
