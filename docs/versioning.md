# Versioning & release

- Semantic Versioning (SEMVER): MAJOR.MINOR.PATCH
- Release flow: tag vX.Y.Z -> CI publishes to NuGet
- Symbols (.snupkg) included; README and XML docs in package
- Changelog maintained in CHANGELOG.md

Recent highlights:
- 0.2.3: Device typed properties and full /devices/{id} mapping
- 0.2.4: Safer ExecuteAction overload (args[], integrationPin?, delay?) and docs updates
