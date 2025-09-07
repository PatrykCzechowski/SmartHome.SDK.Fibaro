using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_ViewVersion_Tests
{
    [Fact]
    public async Task GetDevicesAsync_With_ViewVersion_Appends_Query()
    {
        var json = "[]";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.Matches(@"/devices\?viewVersion=v2$", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var result = await sut.GetDevicesAsync("v2");
        Assert.Empty(result);
        handler.VerifyAll();
    }

    [Fact]
    public async Task GetDeviceAsync_With_ViewVersion_Appends_Query()
    {
        var json = "{ \"id\": 1, \"name\": \"D\" }";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.Matches(@"/devices/1\?viewVersion=v2$", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var device = await sut.GetDeviceAsync(1, "v2");
        Assert.NotNull(device);
        Assert.Equal(1, device!.Id);
    }
}
