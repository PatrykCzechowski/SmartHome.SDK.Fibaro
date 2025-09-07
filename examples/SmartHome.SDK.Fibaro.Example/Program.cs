using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Services;

// Minimal console example using Basic auth from environment variables
// Set environment variables:
//   FIBARO_BASE_URL (e.g., https://your-home-center/api/)
//   FIBARO_USERNAME
//   FIBARO_PASSWORD
// Optional action env vars:
//   FIBARO_ACTION_DEVICE_ID (e.g., 123)
//   FIBARO_ACTION (e.g., turnOn or setValue)
//   FIBARO_ACTION_ARGS (comma-separated, e.g., 50 or 1,true)

var baseUrl = Environment.GetEnvironmentVariable("FIBARO_BASE_URL");
var user = Environment.GetEnvironmentVariable("FIBARO_USERNAME");
var pass = Environment.GetEnvironmentVariable("FIBARO_PASSWORD");

if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
{
    Console.WriteLine("Missing env vars. Set FIBARO_BASE_URL, FIBARO_USERNAME, FIBARO_PASSWORD.");
    return 1;
}

var services = new ServiceCollection();
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri(baseUrl!);
    o.Username = user;
    o.Password = pass;
});

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IFibaroClient>();

Console.WriteLine("Fetching devices...");
var devices = await client.GetDevicesAsync();
Console.WriteLine($"Devices: {devices.Count}");

if (devices.Count > 0)
{
    var first = devices[0];
    Console.WriteLine($"First device: {first.Id} - {first.Name}");
}

// Optional: perform an action if env vars are given
var actionDeviceIdRaw = Environment.GetEnvironmentVariable("FIBARO_ACTION_DEVICE_ID");
var actionName = Environment.GetEnvironmentVariable("FIBARO_ACTION");
var actionArgsRaw = Environment.GetEnvironmentVariable("FIBARO_ACTION_ARGS");

if (!string.IsNullOrWhiteSpace(actionDeviceIdRaw) && !string.IsNullOrWhiteSpace(actionName) && int.TryParse(actionDeviceIdRaw, out var actionDeviceId))
{
    object? parameters = null;
    if (!string.IsNullOrWhiteSpace(actionArgsRaw))
    {
        // very simple comma-split args, all as strings; Fibaro often accepts numbers/strings
        var parts = actionArgsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        parameters = new { args = parts.Cast<object>().ToArray() };
    }

    Console.WriteLine($"Executing action '{actionName}' on device {actionDeviceId}...");
    await client.ExecuteActionAsync(actionDeviceId, actionName!, parameters);
    Console.WriteLine("Action executed.");
}

return 0;
