using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_Device_ById_Tests
{
    [Fact]
    public async Task GetDeviceAsync_Returns_Device()
    {
    var json = "{ \"id\": 10, \"name\": \"Switch\", \"interfaces\": [\"zwave\"], \"properties\": { \"value\": \"true\" }, \"actions\": { \"turnOn\": 0 }, \"created\": 1, \"modified\": 2, \"sortOrder\": 3 }";

        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.EndsWith("/devices/10", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var device = await sut.GetDeviceAsync(10);
        Assert.NotNull(device);
        Assert.Equal(10, device!.Id);
        Assert.Equal("Switch", device.Name);
    Assert.NotNull(device.Interfaces);
    Assert.True(device.Properties!.ContainsKey("value"));
    Assert.True(device.Actions!.ContainsKey("turnOn"));
    }

    [Fact]
    public async Task DeleteDeviceAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Delete, req.Method);
                Assert.EndsWith("/devices/20", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        await sut.DeleteDeviceAsync(20);
        handler.VerifyAll();
    }
}
