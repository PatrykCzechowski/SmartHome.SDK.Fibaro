using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Services;

// Minimal console example using Basic auth from environment variables
// Set environment variables:
//   FIBARO_BASE_URL (e.g., https://your-home-center/api/)
//   FIBARO_USERNAME
//   FIBARO_PASSWORD

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

return 0;
