using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class Device_TypedProperties_Tests
{
    [Fact]
    public async Task TryGetProperty_Converts_Common_Types()
    {
        var json = """
        {
          "id": 1,
          "name": "X",
          "properties": {
            "value": "true",
            "batteryLevel": "100",
            "zwaveVersion": 2.6,
            "obj": { "a": 1 }
          }
        }
        """;

        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var device = await sut.GetDeviceAsync(1);
        Assert.NotNull(device);

        Assert.True(device!.TryGetProperty<bool>("value", out var b) && b);
        Assert.True(device.TryGetProperty<int>("batteryLevel", out var i) && i == 100);
        Assert.True(device.TryGetProperty<double>("zwaveVersion", out var d) && Math.Abs(d - 2.6) < 1e-6);

        var complex = device.GetPropertyOrDefault<dynamic>("obj");
        Assert.NotNull(complex);
    }
}
