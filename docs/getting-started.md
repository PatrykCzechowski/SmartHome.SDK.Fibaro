# Getting started

This guide shows how to install, configure, and make your first requests.

## 1) Install

NuGet package name: `SmartHome.SDK.Fibaro`

## 2) Configure DI

Basic Auth (recommended for Fibaro):

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Services;

var services = new ServiceCollection();
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri("https://your-home-center/api/"); // important: must end with /api/
    o.Username = "apiuser";
    o.Password = "apipass";
});

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IFibaroClient>();
```

Bearer token (if your environment uses it):

```csharp
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri("https://your-home-center/api/");
    o.AccessToken = Environment.GetEnvironmentVariable("FIBARO_TOKEN");
});
```

## 3) First calls

Fetch devices:

```csharp
var devices = await client.GetDevicesAsync();
```

Execute a device action:

```csharp
await client.ExecuteActionAsync(123, "turnOn");
```

Scenes — list, create, execute:

```csharp
var scenes = await client.GetScenesAsync();

var created = await client.CreateSceneAsync(new CreateSceneRequest {
    Name = "My Scene",
    Type = "lua",
    Mode = "manual",
    Content = "-- lua code"
});

await client.ExecuteSceneAsync(created.Id, new ExecuteSceneRequest { AlexaProhibited = false }, pin: "1234");
```

## Notes
- BaseAddress must point to the API and end with `/api/`.
- Use HTTPS in production.
- Error handling: see `error-handling.md` (client throws `FibaroApiException`).
