using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class ExecuteAction_Overload_Tests
{
    [Fact]
    public async Task Posts_To_Correct_Url_With_Args_And_Options()
    {
        HttpRequestMessage? captured = null;
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                captured = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        await sut.ExecuteActionAsync(36, "setValue", new object?[] { 100 }, integrationPin: "1234", delaySeconds: 5);

        Assert.NotNull(captured);
        Assert.Equal(HttpMethod.Post, captured!.Method);
        Assert.Equal("http://localhost/api/devices/36/action/setValue", captured.RequestUri!.ToString());
        var body = await captured.Content!.ReadAsStringAsync();
        Assert.Contains("\"args\":[100]", body);
        Assert.Contains("\"integrationPin\":\"1234\"", body);
        Assert.Contains("\"delay\":5", body);
    }
}
