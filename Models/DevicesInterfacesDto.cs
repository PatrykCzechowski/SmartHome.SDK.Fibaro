namespace SmartHome.SDK.Fibaro.Models;

public sealed class DevicesInterfacesDto
{
    // List of device ids for which to add/delete interfaces
    public List<int>? DevicesId { get; set; }

    // List of interfaces names (e.g., "energy", "inversion")
    public List<string>? Interfaces { get; set; }
}
