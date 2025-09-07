using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Interfaces;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace SmartHome.SDK.Fibaro.Tests;

public class ServiceCollectionExtensions_Auth_Tests
{
    [Fact]
    public async Task AddFibaroClient_WithBearerToken_SetsAuthorizationHeader()
    {
        // Arrange
        var token = "abc123";
        var devices = new[] { new Device { Id = 1, Name = "Test" } };
        var json = JsonSerializer.Serialize(devices);

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns<HttpRequestMessage, CancellationToken>((req, ct) =>
            {
                Assert.True(req.Headers.Authorization != null, "Authorization header should be set");
                Assert.Equal("Bearer", req.Headers.Authorization!.Scheme);
                Assert.Equal(token, req.Headers.Authorization!.Parameter);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                });
            });

        // Wire DI with HttpClientFactory using the mocked handler globally
        var services = new ServiceCollection();
        services.ConfigureAll<HttpClientFactoryOptions>(opt =>
        {
            opt.HttpMessageHandlerBuilderActions.Add(builder =>
            {
                builder.PrimaryHandler = handlerMock.Object;
            });
        });
        services.AddFibaroClient(options =>
        {
            options.BaseAddress = new Uri("http://localhost/api/");
            options.AccessToken = token;
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IFibaroClient>();

        // Act
        var result = await client.GetDevicesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }
}
