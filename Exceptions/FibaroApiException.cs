using System.Net;

namespace SmartHome.SDK.Fibaro.Exceptions;

public class FibaroApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseBody { get; }

    public FibaroApiException(string message, HttpStatusCode statusCode, string? responseBody = null, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
