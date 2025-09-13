using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SmartHome.SDK.Fibaro.Exceptions;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Models;

namespace SmartHome.SDK.Fibaro.Services;

public sealed class FibaroClient : IFibaroClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public FibaroClient(HttpClient httpClient)
    {
        _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        // Expect BaseAddress to be configured externally, e.g., https://host/api/
        _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IReadOnlyList<Device>> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("devices", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get devices: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

    var devices = await response.Content.ReadFromJsonAsync<List<Device>>(_jsonOptions, cancellationToken).ConfigureAwait(false);
    return (IReadOnlyList<Device>)(devices ?? new List<Device>());
    }

    public async Task<IReadOnlyList<Device>> GetDevicesAsync(string viewVersion, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(viewVersion)) throw new ArgumentException("viewVersion cannot be empty", nameof(viewVersion));
        using var response = await _http.GetAsync($"devices?viewVersion={Uri.EscapeDataString(viewVersion)}", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get devices (viewVersion={viewVersion}): {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var devices = await response.Content.ReadFromJsonAsync<List<Device>>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        return (IReadOnlyList<Device>)(devices ?? new List<Device>());
    }

    public async Task<Device?> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"devices/{deviceId}", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        return await response.Content.ReadFromJsonAsync<Device>(_jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Device?> GetDeviceAsync(int deviceId, string viewVersion, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(viewVersion)) throw new ArgumentException("viewVersion cannot be empty", nameof(viewVersion));
        using var response = await _http.GetAsync($"devices/{deviceId}?viewVersion={Uri.EscapeDataString(viewVersion)}", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get device {deviceId} (viewVersion={viewVersion}): {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        return await response.Content.ReadFromJsonAsync<Device>(_jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task ExecuteActionAsync(int deviceId, string action, object? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action cannot be empty", nameof(action));
        var url = $"devices/{deviceId}/action/{Uri.EscapeDataString(action)}";

        // The API accepts JSON body with "args" or parameters array depending on action. We'll send as object.
        var content = parameters is null
            ? JsonContent.Create(new { })
            : JsonContent.Create(parameters);

        using var response = await _http.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to execute action '{action}' on device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task ExecuteActionAsync(int deviceId, string action, IEnumerable<object?> args, string? integrationPin = null, double? delaySeconds = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action cannot be empty", nameof(action));
        if (args is null) throw new ArgumentNullException(nameof(args));

        var url = $"devices/{deviceId}/action/{Uri.EscapeDataString(action)}";

        // Build minimal payload: omit nulls/empties to avoid backend quirks
        var argsArray = args as object?[] ?? args.ToArray();
        var payload = new Dictionary<string, object?>
        {
            ["args"] = argsArray
        };
        if (!string.IsNullOrWhiteSpace(integrationPin)) payload["integrationPin"] = integrationPin;
        if (delaySeconds.HasValue) payload["delay"] = delaySeconds.Value;

        using var response = await _http.PostAsJsonAsync(url, payload, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var resp = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to execute action '{action}' on device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, resp, ex);
        }
    }

    public async Task<IReadOnlyList<Device>> FilterDevicesAsync(DeviceListFiltersDto filters, CancellationToken cancellationToken = default)
    {
        if (filters is null) throw new ArgumentNullException(nameof(filters));

        using var response = await _http.PostAsJsonAsync("devices/filter", filters, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to filter devices: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var devices = await response.Content.ReadFromJsonAsync<List<Device>>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        return (IReadOnlyList<Device>)(devices ?? new List<Device>());
    }

    public async Task<Device> UpdateDeviceAsync(int deviceId, Device device, CancellationToken cancellationToken = default)
    {
        if (device is null) throw new ArgumentNullException(nameof(device));

        using var response = await _http.PutAsJsonAsync($"devices/{deviceId}", device, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to update device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var updated = await response.Content.ReadFromJsonAsync<Device>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (updated is null) throw new FibaroApiException($"Empty response updating device {deviceId}", HttpStatusCode.OK, null);
        return updated;
    }

    public async Task ExecuteGroupActionAsync(string actionName, GroupActionArguments args, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(actionName)) throw new ArgumentException("Action name cannot be empty", nameof(actionName));
        if (args is null) throw new ArgumentNullException(nameof(args));

        using var response = await _http.PostAsJsonAsync($"devices/groupAction/{Uri.EscapeDataString(actionName)}", args, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to execute group action '{actionName}': {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task DeleteDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.DeleteAsync($"devices/{deviceId}", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to delete device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task AddInterfacesToDevicesAsync(DevicesInterfacesDto request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        using var response = await _http.PostAsJsonAsync("devices/addInterface", request, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to add interfaces: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task DeleteInterfacesFromDevicesAsync(DevicesInterfacesDto request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        using var response = await _http.PostAsJsonAsync("devices/deleteInterface", request, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to delete interfaces: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task<Device> AddPollingInterfaceAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PutAsync($"devices/{deviceId}/interfaces/polling", content: null, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to add polling interface to device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var device = await response.Content.ReadFromJsonAsync<Device>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (device is null) throw new FibaroApiException($"Empty response adding polling interface for device {deviceId}", HttpStatusCode.OK, null);
        return device;
    }

    public async Task<Device> DeletePollingInterfaceAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.DeleteAsync($"devices/{deviceId}/interfaces/polling", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to delete polling interface from device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var device = await response.Content.ReadFromJsonAsync<Device>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (device is null) throw new FibaroApiException($"Empty response deleting polling interface for device {deviceId}", HttpStatusCode.OK, null);
        return device;
    }

    public async Task DeleteDelayedActionAsync(long timestamp, int id, CancellationToken cancellationToken = default)
    {
        using var response = await _http.DeleteAsync($"devices/action/{timestamp}/{id}", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to delete delayed action {id} at {timestamp}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task ExecuteActionOnSlaveAsync(string slaveUuid, int deviceId, string action, object? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slaveUuid)) throw new ArgumentException("Slave UUID cannot be empty", nameof(slaveUuid));
        if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action cannot be empty", nameof(action));

        var url = $"slave/{Uri.EscapeDataString(slaveUuid)}/api/devices/{deviceId}/action/{Uri.EscapeDataString(action)}";
        var content = parameters is null ? JsonContent.Create(new { }) : JsonContent.Create(parameters);
        using var response = await _http.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to execute action '{action}' on slave '{slaveUuid}' device {deviceId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task DeleteDeviceOnSlaveAsync(string slaveUuid, int deviceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slaveUuid)) throw new ArgumentException("Slave UUID cannot be empty", nameof(slaveUuid));

        using var response = await _http.DeleteAsync($"slave/{Uri.EscapeDataString(slaveUuid)}/api/devices/{deviceId}", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to delete device {deviceId} on slave '{slaveUuid}': {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task<DeviceTypeHierarchy> GetDevicesHierarchyAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("devices/hierarchy", cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get device type hierarchy: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var tree = await response.Content.ReadFromJsonAsync<DeviceTypeHierarchy>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (tree is null) throw new FibaroApiException("Empty response for device type hierarchy", HttpStatusCode.OK, null);
        return tree;
    }

    public async Task<IReadOnlyList<DeviceInfoDto>> GetUiDeviceInfoAsync(UiDeviceInfoQuery? query = null, CancellationToken cancellationToken = default)
    {
        static void AddParam(List<string> qp, string name, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value)) qp.Add($"{name}={Uri.EscapeDataString(value)}");
        }

        static void AddBoolParam(List<string> qp, string name, bool? value)
        {
            if (value.HasValue) qp.Add($"{name}={(value.Value ? "true" : "false")}");
        }

        var parts = new List<string>();
        if (query is not null)
        {
            if (query.RoomId.HasValue) parts.Add($"roomId={query.RoomId.Value}");
            AddParam(parts, "type", query.Type);
            if (query.Selectors is { Count: > 0 })
            {
                foreach (var s in query.Selectors)
                    AddParam(parts, "selectors", s);
            }
            if (query.Source is { Count: > 0 })
            {
                foreach (var s in query.Source)
                    AddParam(parts, "source", s);
            }
            AddBoolParam(parts, "visible", query.Visible);
            if (query.Classification is { Count: > 0 })
            {
                foreach (var c in query.Classification)
                    AddParam(parts, "classification", c.ToString());
            }
        }

        var url = parts.Count == 0 ? "uiDeviceInfo" : $"uiDeviceInfo?{string.Join("&", parts)}";
        using var response = await _http.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get UI device info: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }

        var list = await response.Content.ReadFromJsonAsync<List<DeviceInfoDto>>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        return (IReadOnlyList<DeviceInfoDto>)(list ?? new List<DeviceInfoDto>());
    }

    // Scenes API
    public async Task<IReadOnlyList<SceneDto>> GetScenesAsync(bool? alexaProhibited = null, CancellationToken cancellationToken = default)
    {
        var url = alexaProhibited.HasValue ? $"scenes?alexaProhibited={(alexaProhibited.Value ? "true" : "false")}" : "scenes";
        using var response = await _http.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get scenes: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        var list = await response.Content.ReadFromJsonAsync<List<SceneDto>>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        return (IReadOnlyList<SceneDto>)(list ?? new List<SceneDto>());
    }

    public async Task<SceneDto?> GetSceneAsync(int sceneId, bool? alexaProhibited = null, CancellationToken cancellationToken = default)
    {
        var url = alexaProhibited.HasValue ? $"scenes/{sceneId}?alexaProhibited={(alexaProhibited.Value ? "true" : "false")}" : $"scenes/{sceneId}";
        using var response = await _http.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to get scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        return await response.Content.ReadFromJsonAsync<SceneDto>(_jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<CreateSceneResponse> CreateSceneAsync(CreateSceneRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        using var response = await _http.PostAsJsonAsync("scenes", request, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to create scene: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        var body = await response.Content.ReadFromJsonAsync<CreateSceneResponse>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (body is null) throw new FibaroApiException("Empty response when creating scene", HttpStatusCode.Created, null);
        return body;
    }

    public async Task UpdateSceneAsync(int sceneId, UpdateSceneRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        using var response = await _http.PutAsJsonAsync($"scenes/{sceneId}", request, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to update scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task DeleteSceneAsync(int sceneId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.DeleteAsync($"scenes/{sceneId}", cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to delete scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task ExecuteSceneAsync(int sceneId, ExecuteSceneRequest? request = null, string? pin = null, CancellationToken cancellationToken = default)
    {
        var url = $"scenes/{sceneId}/execute";
        using var msg = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request ?? new ExecuteSceneRequest())
        };
        if (!string.IsNullOrWhiteSpace(pin)) msg.Headers.Add("Fibaro-User-PIN", pin);
        using var response = await _http.SendAsync(msg, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to execute scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task ExecuteSceneSyncAsync(int sceneId, ExecuteSceneRequest? request = null, string? pin = null, CancellationToken cancellationToken = default)
    {
        var url = $"scenes/{sceneId}/executeSync";
        using var msg = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request ?? new ExecuteSceneRequest())
        };
        if (!string.IsNullOrWhiteSpace(pin)) msg.Headers.Add("Fibaro-User-PIN", pin);
        using var response = await _http.SendAsync(msg, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to execute scene sync {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task<SceneDto> ConvertSceneAsync(int sceneId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsync($"scenes/{sceneId}/convert", content: null, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to convert scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        var dto = await response.Content.ReadFromJsonAsync<SceneDto>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (dto is null) throw new FibaroApiException($"Empty response converting scene {sceneId}", HttpStatusCode.OK, null);
        return dto;
    }

    public async Task<SceneDto> CopySceneAsync(int sceneId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsync($"scenes/{sceneId}/copy", content: null, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to copy scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        var dto = await response.Content.ReadFromJsonAsync<SceneDto>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (dto is null) throw new FibaroApiException($"Empty response copying scene {sceneId}", HttpStatusCode.OK, null);
        return dto;
    }

    public async Task<SceneDto> CopyAndConvertSceneAsync(int sceneId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsync($"scenes/{sceneId}/copyAndConvert", content: null, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to copy and convert scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        var dto = await response.Content.ReadFromJsonAsync<SceneDto>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        if (dto is null) throw new FibaroApiException($"Empty response copy&convert scene {sceneId}", HttpStatusCode.OK, null);
        return dto;
    }

    public async Task KillSceneAsync(int sceneId, string? pin = null, CancellationToken cancellationToken = default)
    {
        var url = $"scenes/{sceneId}/kill";
        using var msg = new HttpRequestMessage(HttpMethod.Post, url);
        if (!string.IsNullOrWhiteSpace(pin)) msg.Headers.Add("Fibaro-User-PIN", pin);
        using var response = await _http.SendAsync(msg, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to kill scene {sceneId}: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
    }

    public async Task<IReadOnlyList<SceneDto>> FilterScenesByTriggersAsync(FilterSceneRequest filters, CancellationToken cancellationToken = default)
    {
        if (filters is null) throw new ArgumentNullException(nameof(filters));
        using var response = await _http.PostAsJsonAsync("scenes/hasTriggers", filters, _jsonOptions, cancellationToken).ConfigureAwait(false);
        try { response.EnsureSuccessStatusCode(); }
        catch (HttpRequestException ex)
        {
            var payload = await SafeReadAsync(response, cancellationToken).ConfigureAwait(false);
            throw new FibaroApiException($"Failed to filter scenes by triggers: {(int)response.StatusCode} {response.ReasonPhrase}", response.StatusCode, payload, ex);
        }
        var list = await response.Content.ReadFromJsonAsync<List<SceneDto>>(_jsonOptions, cancellationToken).ConfigureAwait(false);
        return (IReadOnlyList<SceneDto>)(list ?? new List<SceneDto>());
    }

    private static async Task<string?> SafeReadAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }
}
