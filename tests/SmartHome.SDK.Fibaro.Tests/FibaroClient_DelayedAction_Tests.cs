using System.Net;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_DelayedAction_Tests
{
    [Fact]
    public async Task DeleteDelayedActionAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Delete, req.Method);
                Assert.EndsWith("/devices/action/12345/7", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        await sut.DeleteDelayedActionAsync(12345, 7);
        handler.VerifyAll();
    }
}
