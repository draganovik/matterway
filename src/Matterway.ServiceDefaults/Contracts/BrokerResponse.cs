using System.Net;

namespace Matterway.ServiceDefaults.Contracts;

public sealed record BrokerResponse<T>(
    bool IsSuccess,
    T? Data,
    string? ErrorMessage = null,
    HttpStatusCode? StatusCode = null)
{
    public static BrokerResponse<T> Success(T data, HttpStatusCode? statusCode = null)
    {
        return new BrokerResponse<T>(true, data, null, statusCode);
    }

    public static BrokerResponse<T> Failure(string? errorMessage, HttpStatusCode? statusCode = null)
    {
        return new BrokerResponse<T>(false, default, errorMessage, statusCode);
    }
}