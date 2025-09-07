using System.Net;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_Interfaces_Tests
{
    [Fact]
    public async Task AddInterfacesToDevicesAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/devices/addInterface", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var dto = new DevicesInterfacesDto { DevicesId = new() { 1, 2 }, Interfaces = new() { "energy" } };
        await sut.AddInterfacesToDevicesAsync(dto);
        handler.VerifyAll();
    }

    [Fact]
    public async Task DeleteInterfacesFromDevicesAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/devices/deleteInterface", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var dto = new DevicesInterfacesDto { DevicesId = new() { 3 }, Interfaces = new() { "inversion" } };
        await sut.DeleteInterfacesFromDevicesAsync(dto);
        handler.VerifyAll();
    }
}
