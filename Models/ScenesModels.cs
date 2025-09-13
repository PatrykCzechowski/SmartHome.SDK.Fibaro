using System.Text.Json.Serialization;

namespace SmartHome.SDK.Fibaro.Models;

public sealed class SceneDto
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; } // lua | json | magic | scenario
    [JsonPropertyName("mode")] public string? Mode { get; init; } // automatic | manual
    [JsonPropertyName("icon")] public string? Icon { get; init; }
    [JsonPropertyName("iconExtension")] public string? IconExtension { get; init; }
    [JsonPropertyName("content")] public string? Content { get; init; }
    [JsonPropertyName("maxRunningInstances")] public int? MaxRunningInstances { get; init; }
    [JsonPropertyName("hidden")] public bool? Hidden { get; init; }
    [JsonPropertyName("protectedByPin")] public bool? ProtectedByPin { get; init; }
    // Scenario content structure varies by scene type; keep it flexible
    [JsonPropertyName("scenarioData")] public object? ScenarioData { get; init; }
    [JsonPropertyName("stopOnAlarm")] public bool? StopOnAlarm { get; init; }
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    [JsonPropertyName("restart")] public bool? Restart { get; init; }
    [JsonPropertyName("categories")] public List<int>? Categories { get; init; }
    [JsonPropertyName("created")] public long? Created { get; init; }
    [JsonPropertyName("updated")] public long? Updated { get; init; }
    [JsonPropertyName("isRunning")] public bool? IsRunning { get; init; }
    [JsonPropertyName("isScenarioDataCorrect")] public bool? IsScenarioDataCorrect { get; init; }
    [JsonPropertyName("started")] public long? Started { get; init; }
    [JsonPropertyName("roomId")] public int? RoomId { get; init; }
    [JsonPropertyName("sortOrder")] public int? SortOrder { get; init; }
}

public sealed class CreateSceneRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Mode { get; set; }
    public string? Icon { get; set; }
    public string? Content { get; set; }
    public int? MaxRunningInstances { get; set; }
    public bool? Hidden { get; set; }
    public bool? ProtectedByPin { get; set; }
    public bool? StopOnAlarm { get; set; }
    public bool? Enabled { get; set; }
    public bool? Restart { get; set; } = true;
    public List<int>? Categories { get; set; }
    public object? ScenarioData { get; set; }
    public int? RoomId { get; set; }
}

public sealed class CreateSceneResponse
{
    [JsonPropertyName("id")] public int Id { get; init; }
}

public sealed class UpdateSceneRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Mode { get; set; }
    public int? MaxRunningInstances { get; set; }
    public string? Icon { get; set; }
    public string? Content { get; set; }
    public bool? Hidden { get; set; }
    public bool? ProtectedByPin { get; set; }
    public bool? StopOnAlarm { get; set; }
    public bool? Enabled { get; set; }
    public bool? Restart { get; set; }
    public List<int>? Categories { get; set; }
    public object? ScenarioData { get; set; }
    public int? RoomId { get; set; }
}

public sealed class ExecuteSceneRequest
{
    public bool? AlexaProhibited { get; set; }
    public object? Args { get; set; }
}

public sealed class FilterSceneRule
{
    public string? Type { get; set; }
    public string? Property { get; set; }
    public object? Id { get; set; }
}

public sealed class FilterSceneRequest : List<FilterSceneRule>
{
}
