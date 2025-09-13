using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using SmartHome.SDK.Fibaro.Models;
using SmartHome.SDK.Fibaro.Services;
using Xunit;

namespace SmartHome.SDK.Fibaro.Tests;

public class FibaroClient_History_Tests
{
    [Fact]
    public async Task GetHistoryEvents_Composes_Query()
    {
        var json = "[]";
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                var uri = req.RequestUri!.AbsoluteUri;
                Assert.Contains("/events/history?", uri);
                Assert.Contains("eventType=test", uri);
                Assert.Contains("from=1", uri);
                Assert.Contains("to=2", uri);
                Assert.Contains("sourceType=device", uri);
                Assert.Contains("sourceId=10", uri);
                Assert.Contains("objectType=scene", uri);
                Assert.Contains("objectId=20", uri);
                Assert.Contains("lastId=30", uri);
                Assert.Contains("numberOfRecords=100", uri);
                Assert.Contains("roomId=5", uri);
                Assert.Contains("sectionId=6", uri);
                Assert.Contains("category=7", uri);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        var query = new HistoryEventsQuery
        {
            EventType = "test",
            From = 1,
            To = 2,
            SourceType = "device",
            SourceId = 10,
            ObjectType = "scene",
            ObjectId = 20,
            LastId = 30,
            NumberOfRecords = 100,
            RoomId = 5,
            SectionId = 6,
            Category = 7
        };
        var list = await sut.GetHistoryEventsAsync(query);
        Assert.Empty(list);
        handler.VerifyAll();
    }

    [Fact]
    public async Task DeleteHistoryEvents_Composes_Query()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
            {
                Assert.Equal(HttpMethod.Delete, req.Method);
                var uri = req.RequestUri!.AbsoluteUri;
                Assert.Contains("/events/history?", uri);
                Assert.Contains("eventType=alarm", uri);
                Assert.Contains("timestamp=1000", uri);
                Assert.Contains("shrink=5000", uri);
                Assert.Contains("objectType=device", uri);
                Assert.Contains("objectId=33", uri);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var http = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/api/") };
        var sut = new FibaroClient(http);
        await sut.DeleteHistoryEventsAsync(new DeleteHistoryQuery
        {
            EventType = "alarm",
            Timestamp = 1000,
            Shrink = 5000,
            ObjectType = "device",
            ObjectId = 33
        });
        handler.VerifyAll();
    }
}
