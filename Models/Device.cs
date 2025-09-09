using System.Text.Json;
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
    [JsonPropertyName("remoteGatewayId")] public int? RemoteGatewayId { get; init; }
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    [JsonPropertyName("visible")] public bool? Visible { get; init; }
    [JsonPropertyName("viewXml")] public bool? ViewXml { get; init; }
    [JsonPropertyName("configXml")] public bool? ConfigXml { get; init; }
    [JsonPropertyName("interfaces")] public string[]? Interfaces { get; init; }
    // Properties is a heterogeneous bag depending on device type
    [JsonPropertyName("properties")] public Dictionary<string, JsonElement>? Properties { get; init; }
    // Actions is a dictionary of supported actions and argument count (often 0 or 1)
    [JsonPropertyName("actions")] public Dictionary<string, int>? Actions { get; init; }
    [JsonPropertyName("created")] public long? Created { get; init; }
    [JsonPropertyName("modified")] public long? Modified { get; init; }
    [JsonPropertyName("sortOrder")] public int? SortOrder { get; init; }
}
