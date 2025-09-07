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

See also models in the Models namespace.
