# Changelog

## [0.3.1] - 2025-09-13
### Added
- History Events API: GET /events/history (filtry: eventType, from/to, source/object type/id, lastId, numberOfRecords, room/section/category) oraz DELETE /events/history (eventType, timestamp, shrink, objectType, objectId)
- Modele i testy jednostkowe; dokumentacja w endpoints i usage-examples

## [0.3.0] - 2025-09-13
### Added
- Scenes API support: list/get, create/update/delete, execute/executeSync (PIN header), convert/copy/copyAndConvert, kill, filter by triggers
- Documentation: getting-started, endpoints, scenes guide with examples
- Unit tests for key Scenes flows

## [0.2.4] - 2025-09-12
### Added
- Safer ExecuteAction overload: args[], optional integrationPin and delaySeconds
### Changed
- Docs and README updated with examples and payload notes

## [0.2.3] - 2025-09-09
### Added
- Device: typed property accessors (TryGetProperty/GetPropertyOrDefault) for converting Fibaro string/number formats
- Device: full payload mapping (properties/actions/timestamps)

## [0.2.2] - 2025-09-07
### Changed
- Documentation updates: README and examples refined

## [0.2.1] - 2025-09-07
### Changed
- Docs: add Basic-auth ExecuteAction examples and console example env-var action run
- README: link to action examples and Try it guide

## [0.2.0] - 2025-09-07
### Added
- Comprehensive Devices API coverage: filter, update, delete, actions, group, delayed, interfaces, polling, slave proxy, hierarchy, UI device info
- ViewVersion overloads
- Unit tests across endpoints
 - Documentation in docs/ (see docs/index.md)

## [0.1.0] - YYYY-MM-DD
### Added
- Initial client skeleton, DI, auth, packaging, CI
