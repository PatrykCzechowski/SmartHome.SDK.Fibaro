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

### Scenes

List, create, execute and kill scenes:

```csharp
// All scenes
var scenes = await client.GetScenesAsync();

// Single scene
var scene = await client.GetSceneAsync(10);

// Create a simple LUA scene (content omitted)
var created = await client.CreateSceneAsync(new CreateSceneRequest {
    Name = "My Scene",
    Type = "lua",
    Mode = "manual",
    Content = "-- lua code"
});

// Execute with optional PIN header
await client.ExecuteSceneAsync(created.Id, new ExecuteSceneRequest { AlexaProhibited = false }, pin: "1234");

// Kill if running
await client.KillSceneAsync(created.Id);
```

### History events

Pobranie historii z filtrami i czyszczenie:

```csharp
var events = await client.GetHistoryEventsAsync(new HistoryEventsQuery
{
    EventType = "alarm",
    From = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
    SourceType = "device",
    RoomId = 5,
    NumberOfRecords = 100
});

await client.DeleteHistoryEventsAsync(new DeleteHistoryQuery
{
    EventType = "alarm",
    Timestamp = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds(),
    Shrink = 5000
});
```
