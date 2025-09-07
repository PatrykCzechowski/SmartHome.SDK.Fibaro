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
