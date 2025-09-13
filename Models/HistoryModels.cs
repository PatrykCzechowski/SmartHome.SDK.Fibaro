using System.Text.Json.Serialization;

namespace SmartHome.SDK.Fibaro.Models;

public sealed class HistoryEventObjectRef
{
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("id")] public int? Id { get; init; }
}

public sealed class HistoryEventDto
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("sourceType")] public string? SourceType { get; init; }
    [JsonPropertyName("sourceId")] public int? SourceId { get; init; }
    [JsonPropertyName("timestamp")] public long? Timestamp { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("data")] public object? Data { get; init; }
    [JsonPropertyName("objects")] public IList<HistoryEventObjectRef>? Objects { get; init; }
}

public sealed class HistoryEventsQuery
{
    public string? EventType { get; set; }
    public long? From { get; set; }
    public long? To { get; set; }
    public string? SourceType { get; set; }
    public int? SourceId { get; set; }
    public string? ObjectType { get; set; }
    public int? ObjectId { get; set; }
    public int? LastId { get; set; }
    public int? NumberOfRecords { get; set; }
    public int? RoomId { get; set; }
    public int? SectionId { get; set; }
    public int? Category { get; set; }
}

public sealed class DeleteHistoryQuery
{
    public string? EventType { get; set; }
    public long? Timestamp { get; set; }
    public int? Shrink { get; set; }
    public string? ObjectType { get; set; }
    public int? ObjectId { get; set; }
}
