using System.Text.Json.Serialization;

namespace SmartHome.SDK.Fibaro.Models;

public sealed class Device
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("baseType")] public string? BaseType { get; init; }
    [JsonPropertyName("roomID")] public int? RoomId { get; init; }
    [JsonPropertyName("parentId")] public int? ParentId { get; init; }
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    [JsonPropertyName("visible")] public bool? Visible { get; init; }
    [JsonPropertyName("interfaces")] public string[]? Interfaces { get; init; }

    // Partial projection of DeviceDto from Fibaro API. Extend as needed.
}
