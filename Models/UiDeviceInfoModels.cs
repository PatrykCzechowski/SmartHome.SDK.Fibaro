namespace SmartHome.SDK.Fibaro.Models;

public enum DeviceClassification
{
    Light,
    Valve,
    Drencher,
    OtherBinarySwitch,
    Blind,
    VenetianBlind,
    Awning,
    Curtain,
    Gate,
    LifeDangerSensor,
    SecuritySensor,
    ClimateSensor,
    Meter,
    OtherSensor,
    Thermostat,
    RemoteController,
    Other,
    Sensor,
    None
}

public sealed class DeviceInfoPropertyDto
{
    public IList<object>? Args { get; set; }
    public string? Unit { get; set; }
    public string? UnitPath { get; set; }
    public string? Name { get; set; }
    public string? Label { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
}

public sealed class DeviceInfoDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DeviceClassification? Classification { get; set; }
    public int? RoomId { get; set; }
    public string? Type { get; set; }
    public int? DeviceIcon { get; set; }
    public IList<DeviceInfoPropertyDto>? Properties { get; set; }
    public string? Role { get; set; }
    public IList<DeviceInfoPropertyDto>? Actions { get; set; }
    public IList<object>? Events { get; set; }
}

public sealed class UiDeviceInfoQuery
{
    public int? RoomId { get; set; }
    public string? Type { get; set; }
    public IList<string>? Selectors { get; set; }
    public IList<string>? Source { get; set; }
    public bool? Visible { get; set; }
    public IList<DeviceClassification>? Classification { get; set; }
}
