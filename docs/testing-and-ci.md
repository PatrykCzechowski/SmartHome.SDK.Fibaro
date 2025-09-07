# Testing & CI

- Unit tests: xUnit + Moq (mocking HttpMessageHandler)
- CI: GitHub Actions builds, tests, packs; publishes on tags v*.*.*
- NuGet publish requires NUGET_API_KEY secret

Local:
- dotnet test
- dotnet pack
