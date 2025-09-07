using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_Filter_Update_Tests
{
    [Fact]
    public async Task FilterDevicesAsync_Returns_List()
    {
        var responseJson = "[{ \"id\": 1, \"name\": \"Lamp\" }]";

        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.EndsWith("/devices/filter", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var filters = new DeviceListFiltersDto
        {
            Filters = new List<DeviceListFilterDto>
            {
                new() { Filter = "roomId", Value = new List<object> { 5 } }
            }
        };

        var result = await sut.FilterDevicesAsync(filters);
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Lamp", result[0].Name);
    }

    [Fact]
    public async Task UpdateDeviceAsync_Returns_Updated_Device()
    {
        var body = "{ \"id\": 42, \"name\": \"Updated\" }";

        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Put, req.Method);
                Assert.EndsWith("/devices/42", req.RequestUri!.AbsoluteUri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);

        var updated = await sut.UpdateDeviceAsync(42, new Device { Id = 42, Name = "Updated" });
        Assert.NotNull(updated);
        Assert.Equal(42, updated.Id);
        Assert.Equal("Updated", updated.Name);
    }
}
