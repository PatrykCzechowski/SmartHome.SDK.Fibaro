using System.Net;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_SlaveProxy_Tests
{
    [Fact]
    public async Task ExecuteActionOnSlaveAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/slave/abcd-123/api/devices/11/action/turnOn", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        await sut.ExecuteActionOnSlaveAsync("abcd-123", 11, "turnOn");
        handler.VerifyAll();
    }

    [Fact]
    public async Task DeleteDeviceOnSlaveAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Delete, req.Method);
                Assert.EndsWith("/slave/abcd-123/api/devices/11", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        await sut.DeleteDeviceOnSlaveAsync("abcd-123", 11);
        handler.VerifyAll();
    }
}
