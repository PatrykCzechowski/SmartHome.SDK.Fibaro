# Try it

Run the console example using Basic auth env vars.

PowerShell (Windows):

```powershell
$env:FIBARO_BASE_URL = "https://your-home-center/api/"
$env:FIBARO_USERNAME = "apiuser"
$env:FIBARO_PASSWORD = "apipass"

dotnet run --project .\examples\SmartHome.SDK.Fibaro.Example\SmartHome.SDK.Fibaro.Example.csproj -c Release
```

Notes:
- Use HTTPS
- Account must have required permissions
- Unset variables after testing if needed

Optional: run a device action (e.g., turn on device 123):

```powershell
$env:FIBARO_ACTION_DEVICE_ID = "123"
$env:FIBARO_ACTION = "turnOn"
dotnet run --project .\examples\SmartHome.SDK.Fibaro.Example\SmartHome.SDK.Fibaro.Example.csproj -c Release
```

Or set a value (e.g., dim level to 50):

```powershell
$env:FIBARO_ACTION_DEVICE_ID = "123"
$env:FIBARO_ACTION = "setValue"
$env:FIBARO_ACTION_ARGS = "50"
dotnet run --project .\examples\SmartHome.SDK.Fibaro.Example\SmartHome.SDK.Fibaro.Example.csproj -c Release
```
