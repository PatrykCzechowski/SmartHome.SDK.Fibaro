# Endpoints (API surface)

The IFibaroClient exposes the following methods (mapped to Devices API):

Devices:
- GetDevicesAsync(ct)
- GetDevicesAsync(viewVersion, ct)
- FilterDevicesAsync(filters, ct)
- GetDeviceAsync(id, ct)
- GetDeviceAsync(id, viewVersion, ct)
- UpdateDeviceAsync(id, device, ct)
- DeleteDeviceAsync(id, ct)

Actions:
- ExecuteActionAsync(deviceId, action, parameters?, ct)
- ExecuteActionAsync(deviceId, action, args[], integrationPin?, delaySeconds?, ct)
- ExecuteGroupActionAsync(actionName, args, ct)
- DeleteDelayedActionAsync(timestamp, id, ct)

Interfaces:
- AddInterfacesToDevicesAsync(request, ct)
- DeleteInterfacesFromDevicesAsync(request, ct)
- AddPollingInterfaceAsync(deviceId, ct)
- DeletePollingInterfaceAsync(deviceId, ct)

Proxy (slave):
- ExecuteActionOnSlaveAsync(slaveUuid, deviceId, action, parameters?, ct)
- DeleteDeviceOnSlaveAsync(slaveUuid, deviceId, ct)

Other:
- GetDevicesHierarchyAsync(ct)
- GetUiDeviceInfoAsync(query?, ct)

Scenes:
- GetScenesAsync(alexaProhibited?, ct)
- GetSceneAsync(sceneId, alexaProhibited?, ct)
- CreateSceneAsync(request, ct)
- UpdateSceneAsync(sceneId, request, ct)
- DeleteSceneAsync(sceneId, ct)
- ExecuteSceneAsync(sceneId, request?, pin?, ct)
- ExecuteSceneSyncAsync(sceneId, request?, pin?, ct)
- ConvertSceneAsync(sceneId, ct)
- CopySceneAsync(sceneId, ct)
- CopyAndConvertSceneAsync(sceneId, ct)
- KillSceneAsync(sceneId, pin?, ct)
- FilterScenesByTriggersAsync(filters, ct)

History Events:
- GetHistoryEventsAsync(query?, ct)
- DeleteHistoryEventsAsync(query?, ct)

See also models in the Models namespace.

Notes on ExecuteAction payload:
- The API expects DeviceActionArgumentsDto: { args: [], integrationPin?: string, delay?: number }.
- Do NOT include deviceId/action in the body (they are in the URL).

Notes on Scenes execute:
- `pin` is sent in header `Fibaro-User-PIN` for POST variants.
- `ExecuteSceneRequest` supports `alexaProhibited` and optional `args` object.
