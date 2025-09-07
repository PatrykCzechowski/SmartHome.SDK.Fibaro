using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_Hierarchy_Tests
{
    [Fact]
    public async Task GetDevicesHierarchyAsync_Returns_Tree()
    {
        var json = "{ \"type\": \"com.fibaro.device\", \"children\": [{ \"type\": \"com.fibaro.zwaveDevice\", \"children\": [] }] }";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.EndsWith("/devices/hierarchy", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var tree = await sut.GetDevicesHierarchyAsync();
        Assert.Equal("com.fibaro.device", tree.Type);
        Assert.NotNull(tree.Children);
        Assert.Single(tree.Children!);
        Assert.Equal("com.fibaro.zwaveDevice", tree.Children![0].Type);
    }
}
