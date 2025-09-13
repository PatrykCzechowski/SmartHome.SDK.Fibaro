using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_Scenes_Tests
{
    [Fact]
    public async Task GetScenesAsync_Returns_List()
    {
        var json = "[{ \"id\": 1, \"name\": \"Scene A\" }]";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.EndsWith("/scenes", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var list = await sut.GetScenesAsync();
        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
    }

    [Fact]
    public async Task CreateSceneAsync_Returns_Id()
    {
        var json = "{ \"id\": 42 }";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/scenes", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var resp = await sut.CreateSceneAsync(new CreateSceneRequest { Name = "X", Type = "lua" });
        Assert.Equal(42, resp.Id);
    }

    [Fact]
    public async Task ExecuteSceneAsync_Sends_Header_Pin_When_Provided()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/scenes/7/execute", req.RequestUri!.AbsoluteUri);
                Assert.True(req.Headers.Contains("Fibaro-User-PIN"));
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        await sut.ExecuteSceneAsync(7, new ExecuteSceneRequest { AlexaProhibited = true }, pin: "1234");
        handler.VerifyAll();
    }
}
