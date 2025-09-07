# Usage examples (cookbook)

Turn on a device:
await client.ExecuteActionAsync(123, "turnOn");

Set value (e.g., dim level):
await client.ExecuteActionAsync(123, "setValue", new { args = new object[] { 50 } });

Group action (turn off all lights in room 5):
await client.ExecuteGroupActionAsync("turnOff", new GroupActionArguments
{
    Filters = new DeviceListFiltersDto
    {
        Filters = new() { new DeviceListFilterDto { Filter = "roomId", Value = new() { 5 } } }
    }
});

Filter devices by type:
var list = await client.FilterDevicesAsync(new DeviceListFiltersDto
{
    Filters = new() { new DeviceListFilterDto { Filter = "type", Value = new() { "com.fibaro.light" } } }
});

UI Device info (visible lights):
var info = await client.GetUiDeviceInfoAsync(new UiDeviceInfoQuery
{
    Type = "com.fibaro.light",
    Visible = true
});
