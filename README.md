# SmartHome.SDK.Fibaro

A lightweight .NET client SDK for Fibaro/Home Center REST API.

## Install

NuGet package name: `SmartHome.SDK.Fibaro`

```powershell
Install-Package SmartHome.SDK.Fibaro
```

## Quickstart

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Services;

var services = new ServiceCollection();

// Option A: Bearer token
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri("https://your-home-center/api/");
    o.AccessToken = "<token>";
});

// Option B: Basic auth
// services.AddFibaroClient(o =>
// {
//     o.BaseAddress = new Uri("https://your-home-center/api/");
//     o.Username = "apiuser";
//     o.Password = "apipass";
// });

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IFibaroClient>();

var devices = await client.GetDevicesAsync();
await client.ExecuteActionAsync(123, "turnOn");
```

## Features
- HttpClient-based, DI-friendly typed client
- Authentication: Bearer token or Basic
- Error handling with `FibaroApiException` (status code + payload)
- Models mapped with `System.Text.Json`

## API Surface
- `Task<IReadOnlyList<Device>> GetDevicesAsync(CancellationToken ct = default)`
- `Task ExecuteActionAsync(int deviceId, string action, object? parameters = null, CancellationToken ct = default)`

## Testing

Project includes xUnit + Moq tests. To run:

```powershell
# From repo root
Dotnet restore
Dotnet build -c Release
Dotnet test .\tests\SmartHome.SDK.Fibaro.Tests\SmartHome.SDK.Fibaro.Tests.csproj -c Release
```

## Packaging

Pack the library (README and XML docs included):

```powershell
Dotnet pack .\SmartHome.SDK.Fibaro.csproj -c Release
```

## CI/CD

This repo includes a GitHub Actions workflow to build/test on push and publish on version tags. Configure the secret `NUGET_API_KEY` in your repository settings.

## License

MIT
