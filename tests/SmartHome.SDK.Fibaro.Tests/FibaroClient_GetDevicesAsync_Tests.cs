using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_GetDevicesAsync_Tests
{
    [Fact]
    public async Task GetDevicesAsync_ReturnsDevices()
    {
        // Arrange
        var json = "[ { \"id\": 101, \"name\": \"Lamp\", \"type\": \"com.fibaro.Fake\" } ]";

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.EndsWith("/devices", request.RequestUri!.AbsoluteUri.TrimEnd('/'));

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                return response;
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/api/", UriKind.Absolute)
        };

        var sut = new FibaroClient(httpClient);

        // Act
        var devices = await sut.GetDevicesAsync();

        // Assert
        Assert.NotNull(devices);
        var list = devices.ToList();
        Assert.Single(list);
        Assert.Equal(101, list[0].Id);
        Assert.Equal("Lamp", list[0].Name);

        handlerMock.VerifyAll();
    }
}
