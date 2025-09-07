using Microsoft.Extensions.DependencyInjection;
using SmartHome.SDK.Fibaro.Interfaces;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace SmartHome.SDK.Fibaro.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers FibaroClient typed HttpClient.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configure">Optional configuration of HttpClient (BaseAddress, auth, etc.).</param>
    public static IServiceCollection AddFibaroClient(this IServiceCollection services, Action<HttpClient>? configure = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddHttpClient<IFibaroClient, FibaroClient>(client =>
        {
            client.BaseAddress ??= new Uri("http://localhost/api/");
            configure?.Invoke(client);
        });

        return services;
    }

    /// <summary>
    /// Registers IFibaroClient with options-driven configuration, including authentication.
    /// </summary>
    public static IServiceCollection AddFibaroClient(this IServiceCollection services, Action<FibaroClientOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.AddHttpClient<IFibaroClient, FibaroClient>((sp, client) =>
        {
            var options = new FibaroClientOptions();
            configure(options);

            // Base address
            client.BaseAddress = options.BaseAddress ?? new Uri("http://localhost/api/");

            // Auth: prefer Bearer if provided, otherwise Basic if both username/password exist
            if (!string.IsNullOrWhiteSpace(options.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.AccessToken);
            }
            else if (!string.IsNullOrWhiteSpace(options.Username) && options.Password is not null)
            {
                var raw = Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}");
                var basic = Convert.ToBase64String(raw);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);
            }
        });

        return services;
    }
}
