# Getting started

This guide shows how to install, configure, and make your first request.

1. Install the NuGet package:
   - Package: SmartHome.SDK.Fibaro
2. Configure DI

Example:

using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Services;

var services = new ServiceCollection();

// Basic auth (recommended for Fibaro)
services.AddFibaroClient(o =>
{
    o.BaseAddress = new Uri("https://your-home-center/api/");
    o.Username = "apiuser";
    o.Password = "apipass";
});

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IFibaroClient>();

var devices = await client.GetDevicesAsync();

Notes:
- BaseAddress must end with /api/
- Use HTTPS in production
