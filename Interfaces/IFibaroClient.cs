using SmartHome.SDK.Fibaro.Models;

namespace SmartHome.SDK.Fibaro.Interfaces;

public interface IFibaroClient
{
    /// <summary>
    /// Returns all available devices.
    /// </summary>
    Task<IReadOnlyList<Device>> GetDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action on a device (e.g., turnOn, setValue, etc.).
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="action">Action name as defined by Fibaro API.</param>
    /// <param name="parameters">Optional parameters for the action.</param>
    /// <param name="cancellationToken">Token to observe while waiting for the request to complete.</param>
    Task ExecuteActionAsync(int deviceId, string action, object? parameters = null, CancellationToken cancellationToken = default);
}
