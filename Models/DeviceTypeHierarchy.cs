namespace SmartHome.SDK.Fibaro.Models;

public sealed class DeviceTypeHierarchy
{
    public string? Type { get; set; }
    public List<DeviceTypeHierarchy>? Children { get; set; }
}
