namespace SmartHome.SDK.Fibaro.Models;

public sealed class GroupActionArguments
{
    // Filters to select target devices; shape can mirror DeviceListFiltersDto
    public DeviceListFiltersDto? Filters { get; set; }

    // Action arguments, loosely typed
    public List<object>? Args { get; set; }

    // Optional delay in seconds and integration pin
    public double? Delay { get; set; }
    public string? IntegrationPin { get; set; }
}
