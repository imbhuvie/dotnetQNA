using System.Net;
using System.Net.Http;
using TechnicalMastery.Wpf.Models;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// Thrown when the API answers with success=false or an HTTP error status.
/// Carries the server's message so view models can show it directly.
/// </summary>
public class ApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public ApiException(string message, HttpStatusCode? statusCode = null)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public static string UserMessage(Exception ex)
    {
        if (ex is ApiException api && !string.IsNullOrWhiteSpace(api.Message))
        {
            return api.Message;
        }

        if (ex is HttpRequestException)
        {
            return "Unable to connect to the server. Please check that the API is running and try again.";
        }

        if (ex is TaskCanceledException)
        {
            return "The request timed out. Please try again.";
        }

        return "Something went wrong. Please try again.";
    }
}
