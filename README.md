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

// Basic auth (recommended for Fibaro/Home Center)
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri("https://your-home-center/api/");
    o.Username = "apiuser";
    o.Password = "apipass";
});

// Or: Bearer token
// services.AddFibaroClient(o =>
// {
//     o.BaseAddress = new Uri("https://your-home-center/api/");
//     o.AccessToken = "<token>";
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
- `Task<Device?> GetDeviceAsync(int deviceId, CancellationToken ct = default)`
- `Task ExecuteActionAsync(int deviceId, string action, object? parameters = null, CancellationToken ct = default)`
- `Task DeleteDeviceAsync(int deviceId, CancellationToken ct = default)`
- `Task<IReadOnlyList<Device>> FilterDevicesAsync(DeviceListFiltersDto filters, CancellationToken ct = default)`
- `Task<Device> UpdateDeviceAsync(int deviceId, Device device, CancellationToken ct = default)`
- `Task ExecuteGroupActionAsync(string actionName, GroupActionArguments args, CancellationToken ct = default)`
- `Task AddInterfacesToDevicesAsync(DevicesInterfacesDto request, CancellationToken ct = default)`
- `Task DeleteInterfacesFromDevicesAsync(DevicesInterfacesDto request, CancellationToken ct = default)`
- `Task<Device> AddPollingInterfaceAsync(int deviceId, CancellationToken ct = default)`
- `Task<Device> DeletePollingInterfaceAsync(int deviceId, CancellationToken ct = default)`
- `Task DeleteDelayedActionAsync(long timestamp, int id, CancellationToken ct = default)`
- `Task ExecuteActionOnSlaveAsync(string slaveUuid, int deviceId, string action, object? parameters = null, CancellationToken ct = default)`
- `Task DeleteDeviceOnSlaveAsync(string slaveUuid, int deviceId, CancellationToken ct = default)`
- `Task<DeviceTypeHierarchy> GetDevicesHierarchyAsync(CancellationToken ct = default)`
- `Task<IReadOnlyList<DeviceInfoDto>> GetUiDeviceInfoAsync(UiDeviceInfoQuery? query = null, CancellationToken ct = default)`

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

## Documentation

See the `docs/` folder:
- `docs/getting-started.md` – install and first request
- `docs/authentication.md` – Basic and Bearer setup
- `docs/endpoints.md` – full API surface
- `docs/usage-examples.md` – cookbook
- `docs/error-handling.md` – FibaroApiException and patterns
- `docs/testing-and-ci.md` – tests and CI
- `docs/troubleshooting.md` – common issues
- `docs/versioning.md` – release process
- `docs/try-it.md` – quick run example

## License

MIT
