# Scenes

This page documents Scenes support in the SDK and shows practical examples.

## Overview

The SDK maps Fibaro Scenes endpoints and provides these methods on `IFibaroClient`:

- `GetScenesAsync(alexaProhibited?, ct)`
- `GetSceneAsync(sceneId, alexaProhibited?, ct)`
- `CreateSceneAsync(request, ct)` → returns `{ id }`
- `UpdateSceneAsync(sceneId, request, ct)`
- `DeleteSceneAsync(sceneId, ct)`
- `ExecuteSceneAsync(sceneId, request?, pin?, ct)`
- `ExecuteSceneSyncAsync(sceneId, request?, pin?, ct)`
- `ConvertSceneAsync(sceneId, ct)`
- `CopySceneAsync(sceneId, ct)`
- `CopyAndConvertSceneAsync(sceneId, ct)`
- `KillSceneAsync(sceneId, pin?, ct)`
- `FilterScenesByTriggersAsync(filters, ct)`

Models:
- `SceneDto`
- `CreateSceneRequest`, `CreateSceneResponse`
- `UpdateSceneRequest`
- `ExecuteSceneRequest` (supports `alexaProhibited` and arbitrary `args`)
- `FilterSceneRequest` (array of simple rules)

## Creating scenes

```csharp
var created = await client.CreateSceneAsync(new CreateSceneRequest {
    Name = "Wake up",
    Type = "lua", // or json/magic/scenario
    Mode = "manual", // or automatic
    Content = "-- LUA content here",
    Hidden = false,
    Enabled = true,
    Restart = true
});
```

Update:

```csharp
await client.UpdateSceneAsync(created.Id, new UpdateSceneRequest {
    Name = "Wake up (updated)",
    Enabled = true
});
```

Delete:

```csharp
await client.DeleteSceneAsync(created.Id);
```

## Executing scenes

Execute asynchronously. If scene is protected provide `pin` (header `Fibaro-User-PIN`):

```csharp
await client.ExecuteSceneAsync(created.Id, new ExecuteSceneRequest {
    AlexaProhibited = false,
    Args = new { brightness = 60 } // optional payload
}, pin: "1234");
```

Execute synchronously:

```csharp
await client.ExecuteSceneSyncAsync(created.Id, new ExecuteSceneRequest());
```

Kill running scene:

```csharp
await client.KillSceneAsync(created.Id, pin: "1234");
```

## Convert/Copy

```csharp
var lua = await client.ConvertSceneAsync(created.Id);
var copy = await client.CopySceneAsync(created.Id);
var copyLua = await client.CopyAndConvertSceneAsync(created.Id);
```

## Filter by triggers

```csharp
var list = await client.FilterScenesByTriggersAsync(new FilterSceneRequest
{
    new() { Type = "device", Property = "value", Id = 1 }
});
```

## Tips
- `args`/`scenarioData` are flexible; the SDK uses `object` to keep compatibility.
- Errors throw `FibaroApiException` with status and response body, see error-handling.md.
- Ensure the client BaseAddress ends with `/api/`.
