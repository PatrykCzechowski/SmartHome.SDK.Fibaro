using System.Net;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_GroupAction_Tests
{
    [Fact]
    public async Task ExecuteGroupActionAsync_Succeeds()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/devices/groupAction/turnOff", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var args = new GroupActionArguments
        {
            Filters = new DeviceListFiltersDto
            {
                Filters = new List<DeviceListFilterDto> { new() { Filter = "type", Value = new List<object> { "com.fibaro.light" } } }
            },
            Args = new List<object>(),
            Delay = null,
            IntegrationPin = null
        };

        await sut.ExecuteGroupActionAsync("turnOff", args);
        handler.VerifyAll();
    }
}
