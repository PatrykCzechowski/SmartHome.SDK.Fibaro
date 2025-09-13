using SmartHome.SDK.Fibaro.Models;

namespace SmartHome.SDK.Fibaro.Interfaces;

public interface IFibaroClient
{
    /// <summary>
    /// Returns all available devices.
    /// </summary>
    Task<IReadOnlyList<Device>> GetDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all available devices for a specific UI view version (e.g., v2).
    /// </summary>
    Task<IReadOnlyList<Device>> GetDevicesAsync(string viewVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single device by id.
    /// </summary>
    Task<Device?> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single device by id for a specific UI view version (e.g., v2).
    /// </summary>
    Task<Device?> GetDeviceAsync(int deviceId, string viewVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns devices filtered by given filters.
    /// </summary>
    Task<IReadOnlyList<Device>> FilterDevicesAsync(DeviceListFiltersDto filters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a group action with filters and arguments.
    /// </summary>
    Task ExecuteGroupActionAsync(string actionName, GroupActionArguments args, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action on a device (e.g., turnOn, setValue, etc.).
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="action">Action name as defined by Fibaro API.</param>
    /// <param name="parameters">Optional parameters for the action.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the request to complete.</param>
    Task ExecuteActionAsync(int deviceId, string action, object? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action on a device using strongly-typed arguments array and optional integration PIN and delay.
    /// This maps directly to DeviceActionArgumentsDto: { args, integrationPin, delay }.
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="action">Action name as defined by Fibaro API (e.g., setValue, turnOn).</param>
    /// <param name="args">Action arguments array (may be empty for actions like turnOn).</param>
    /// <param name="integrationPin">Optional integration pin for protected actions.</param>
    /// <param name="delaySeconds">Optional action delay in seconds.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the request to complete.</param>
    Task ExecuteActionAsync(int deviceId, string action, IEnumerable<object?> args, string? integrationPin = null, double? delaySeconds = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing device with provided data (partial/complete per API contract).
    /// </summary>
    Task<Device> UpdateDeviceAsync(int deviceId, Device device, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a device by id.
    /// </summary>
    Task DeleteDeviceAsync(int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds interfaces to devices.
    /// </summary>
    Task AddInterfacesToDevicesAsync(DevicesInterfacesDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes interfaces from devices.
    /// </summary>
    Task DeleteInterfacesFromDevicesAsync(DevicesInterfacesDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds polling interface to a device.
    /// </summary>
    Task<Device> AddPollingInterfaceAsync(int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes polling interface from a device.
    /// </summary>
    Task<Device> DeletePollingInterfaceAsync(int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a delayed action by timestamp and id.
    /// </summary>
    Task DeleteDelayedActionAsync(long timestamp, int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action on a device via master as proxy for slave.
    /// </summary>
    Task ExecuteActionOnSlaveAsync(string slaveUuid, int deviceId, string action, object? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a device on a slave via master proxy.
    /// </summary>
    Task DeleteDeviceOnSlaveAsync(string slaveUuid, int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets device type hierarchy.
    /// </summary>
    Task<DeviceTypeHierarchy> GetDevicesHierarchyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets UI device info with optional filters.
    /// </summary>
    Task<IReadOnlyList<DeviceInfoDto>> GetUiDeviceInfoAsync(UiDeviceInfoQuery? query = null, CancellationToken cancellationToken = default);

    // Scenes API
    Task<IReadOnlyList<SceneDto>> GetScenesAsync(bool? alexaProhibited = null, CancellationToken cancellationToken = default);
    Task<SceneDto?> GetSceneAsync(int sceneId, bool? alexaProhibited = null, CancellationToken cancellationToken = default);
    Task<CreateSceneResponse> CreateSceneAsync(CreateSceneRequest request, CancellationToken cancellationToken = default);
    Task UpdateSceneAsync(int sceneId, UpdateSceneRequest request, CancellationToken cancellationToken = default);
    Task DeleteSceneAsync(int sceneId, CancellationToken cancellationToken = default);

    // Execute scene (async and sync variants; pin can be header or query as per API)
    Task ExecuteSceneAsync(int sceneId, ExecuteSceneRequest? request = null, string? pin = null, CancellationToken cancellationToken = default);
    Task ExecuteSceneSyncAsync(int sceneId, ExecuteSceneRequest? request = null, string? pin = null, CancellationToken cancellationToken = default);

    // Additional helpers from API
    Task<SceneDto> ConvertSceneAsync(int sceneId, CancellationToken cancellationToken = default);
    Task<SceneDto> CopySceneAsync(int sceneId, CancellationToken cancellationToken = default);
    Task<SceneDto> CopyAndConvertSceneAsync(int sceneId, CancellationToken cancellationToken = default);
    Task KillSceneAsync(int sceneId, string? pin = null, CancellationToken cancellationToken = default);

    // Filter scenes by triggers
    Task<IReadOnlyList<SceneDto>> FilterScenesByTriggersAsync(FilterSceneRequest filters, CancellationToken cancellationToken = default);
}
