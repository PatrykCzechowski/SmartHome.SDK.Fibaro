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
