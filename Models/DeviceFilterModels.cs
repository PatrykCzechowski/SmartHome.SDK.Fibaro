namespace SmartHome.SDK.Fibaro.Models;

public sealed class DeviceListFiltersDto
{
    public List<DeviceListFilterDto>? Filters { get; set; }

    // Attributes are flexible; for now support a map (e.g., { "main": ["string"] })
    public Dictionary<string, List<string>>? Attributes { get; set; }
}

public sealed class DeviceListFilterDto
{
    public string? Filter { get; set; }
    public List<object>? Value { get; set; }
}
