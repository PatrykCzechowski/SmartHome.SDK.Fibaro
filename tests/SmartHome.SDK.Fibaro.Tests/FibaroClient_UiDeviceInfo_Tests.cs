using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_UiDeviceInfo_Tests
{
    [Fact]
    public async Task GetUiDeviceInfoAsync_NoQuery_Returns_List()
    {
        var json = "[{ \"id\": 1, \"name\": \"Device\" }]";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.EndsWith("/uiDeviceInfo", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var result = await sut.GetUiDeviceInfoAsync();
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task GetUiDeviceInfoAsync_WithQuery_Composes_Params()
    {
        var json = "[]";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                var uri = req.RequestUri!.AbsoluteUri;
                Assert.Contains("/uiDeviceInfo?", uri);
                Assert.Contains("roomId=5", uri);
                Assert.Contains("type=com.fibaro.light", uri);
                Assert.Contains("selectors=properties", uri);
                Assert.Contains("selectors=actions", uri);
                Assert.Contains("source=scene", uri);
                Assert.Contains("visible=true", uri);
                Assert.Contains("classification=Light", uri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var query = new UiDeviceInfoQuery
        {
            RoomId = 5,
            Type = "com.fibaro.light",
            Selectors = new List<string> { "properties", "actions" },
            Source = new List<string> { "scene" },
            Visible = true,
            Classification = new List<DeviceClassification> { DeviceClassification.Light }
        };
        var result = await sut.GetUiDeviceInfoAsync(query);
        Assert.Empty(result);
        handler.VerifyAll();
    }
}
