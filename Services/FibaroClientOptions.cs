using System;

namespace SmartHome.SDK.Fibaro.Services;

/// <summary>
/// Options to configure Fibaro HTTP client (base address and authentication).
/// </summary>
public sealed class FibaroClientOptions
{
    /// <summary>
    /// Base address to the Fibaro API, e.g. https://homecenter/api/
    /// </summary>
    public Uri? BaseAddress { get; set; }

    /// <summary>
    /// Bearer access token. If set, client will send Authorization: Bearer {token}.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Username for basic authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for basic authentication.
    /// </summary>
    public string? Password { get; set; }
}
