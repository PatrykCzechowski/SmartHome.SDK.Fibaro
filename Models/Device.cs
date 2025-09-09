using System.Globalization;
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

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Tries to read a property value as a specific type, converting common Fibaro string-encoded values.
    /// Supports: string, bool, int, long, double, decimal, enums, and complex objects via JSON deserialization.
    /// </summary>
    public bool TryGetProperty<T>(string name, out T? value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(name) || Properties is null || !Properties.TryGetValue(name, out var el))
            return false;

        var target = typeof(T);

        // Null/Undefined handling
        if (el.ValueKind == JsonValueKind.Null || el.ValueKind == JsonValueKind.Undefined)
            return false;

        try
        {
            if (target == typeof(string))
            {
                var s = el.ValueKind == JsonValueKind.String ? el.GetString() : el.ToString();
                value = (T?)(object?)s;
                return true;
            }
            if (target == typeof(bool) || target == typeof(bool?))
            {
                bool b;
                if (el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False)
                    b = el.GetBoolean();
                else if (el.ValueKind == JsonValueKind.Number)
                    b = el.GetDouble() != 0d;
                else if (el.ValueKind == JsonValueKind.String)
                {
                    var s = el.GetString();
                    if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1" || string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase)) b = true;
                    else if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || s == "0" || string.Equals(s, "no", StringComparison.OrdinalIgnoreCase)) b = false;
                    else if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dn)) b = Math.Abs(dn) > double.Epsilon;
                    else return false;
                }
                else return false;

                value = (T?)(object)b;
                return true;
            }
            if (target == typeof(int) || target == typeof(int?))
            {
                int n;
                if (el.ValueKind == JsonValueKind.Number) { if (!el.TryGetInt32(out n)) n = (int)el.GetDouble(); }
                else if (el.ValueKind == JsonValueKind.String)
                {
                    if (!int.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out n)) return false;
                }
                else return false;
                value = (T?)(object)n;
                return true;
            }
            if (target == typeof(long) || target == typeof(long?))
            {
                long n;
                if (el.ValueKind == JsonValueKind.Number) { if (!el.TryGetInt64(out n)) n = (long)el.GetDouble(); }
                else if (el.ValueKind == JsonValueKind.String)
                {
                    if (!long.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out n)) return false;
                }
                else return false;
                value = (T?)(object)n;
                return true;
            }
            if (target == typeof(double) || target == typeof(double?))
            {
                double d;
                if (el.ValueKind == JsonValueKind.Number) d = el.GetDouble();
                else if (el.ValueKind == JsonValueKind.String)
                {
                    if (!double.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return false;
                }
                else return false;
                value = (T?)(object)d;
                return true;
            }
            if (target == typeof(decimal) || target == typeof(decimal?))
            {
                decimal d;
                if (el.ValueKind == JsonValueKind.Number) d = el.GetDecimal();
                else if (el.ValueKind == JsonValueKind.String)
                {
                    if (!decimal.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return false;
                }
                else return false;
                value = (T?)(object)d;
                return true;
            }
            if (target.IsEnum)
            {
                var s = el.ValueKind == JsonValueKind.String ? el.GetString() : el.ToString();
                if (s is null) return false;
                if (Enum.TryParse(target, s, true, out var parsed))
                {
                    value = (T?)parsed;
                    return true;
                }
                return false;
            }

            // Complex types: deserialize
            var obj = el.Deserialize<T>(JsonOptions);
            if (obj is null) return false;
            value = obj;
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Gets a property converted to T or returns the provided default value if conversion fails.
    /// </summary>
    public T? GetPropertyOrDefault<T>(string name, T? defaultValue = default)
        => TryGetProperty<T>(name, out var v) ? v : defaultValue;
}
