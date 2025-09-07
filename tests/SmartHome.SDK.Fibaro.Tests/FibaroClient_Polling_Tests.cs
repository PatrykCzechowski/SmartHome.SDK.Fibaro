using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_Polling_Tests
{
    [Fact]
    public async Task AddPollingInterfaceAsync_Returns_Device()
    {
        var json = "{ \"id\": 9, \"name\": \"Sensor\" }";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Put, req.Method);
                Assert.EndsWith("/devices/9/interfaces/polling", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var device = await sut.AddPollingInterfaceAsync(9);
        Assert.Equal(9, device.Id);
        Assert.Equal("Sensor", device.Name);
    }

    [Fact]
    public async Task DeletePollingInterfaceAsync_Returns_Device()
    {
        var json = "{ \"id\": 9, \"name\": \"Sensor\" }";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Delete, req.Method);
                Assert.EndsWith("/devices/9/interfaces/polling", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var device = await sut.DeletePollingInterfaceAsync(9);
        Assert.Equal(9, device.Id);
        Assert.Equal("Sensor", device.Name);
    }
}
