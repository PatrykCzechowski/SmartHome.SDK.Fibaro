# Usage examples (cookbook)

Basic auth client setup (recommended for Fibaro):

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Services;

var services = new ServiceCollection();
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri("https://your-home-center/api/");
    o.Username = "apiuser";
    o.Password = "apipass";
});
var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IFibaroClient>();
```

Execute a device action (turn on):

```csharp
await client.ExecuteActionAsync(123, "turnOn");
```

Set value (e.g., dim level to 50):

```csharp
// Safer overload mapping to { args, integrationPin?, delay? }
await client.ExecuteActionAsync(123, "setValue", new object?[] { 50 });
// With PIN and delay (optional):
await client.ExecuteActionAsync(123, "setValue", new object?[] { 50 }, integrationPin: "1234", delaySeconds: 30);
```

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
