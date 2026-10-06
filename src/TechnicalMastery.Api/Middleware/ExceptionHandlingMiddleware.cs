using System.Net;
using System.Text.Json;
using FluentValidation;
using TechnicalMastery.Application.Common.Exceptions;
using TechnicalMastery.Application.DTOs.Common;

namespace TechnicalMastery.Api.Middleware;

/// <summary>
/// Converts service-layer exceptions into the uniform <see cref="ApiResponse{T}"/>
/// envelope with the correct HTTP status code. Technical details (stack traces)
/// are logged server-side and never sent to the client (§20).
/// <para>Mapping: NotFound → 404, Conflict → 409, Validation/Argument → 400, other → 500.</para>
/// </summary>
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        (HttpStatusCode statusCode, string message, List<string> errors) = Map(exception);

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            this.logger.LogError(exception, "Unhandled exception processing {Method} {Path}.", context.Request.Method, context.Request.Path);
        }
        else
        {
            this.logger.LogWarning("Handled {ExceptionType} on {Method} {Path}: {Message}.", exception.GetType().Name, context.Request.Method, context.Request.Path, message);
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        ApiResponse<object> response = ApiResponse<object>.Fail(message, errors);
        string json = JsonSerializer.Serialize(response, JsonOptions);

        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode StatusCode, string Message, List<string> Errors) Map(Exception exception)
    {
        if (exception is NotFoundException notFound)
        {
            return (HttpStatusCode.NotFound, notFound.Message, new List<string>());
        }

        if (exception is ConflictException conflict)
        {
            return (HttpStatusCode.Conflict, conflict.Message, new List<string>());
        }

        if (exception is ValidationException validation)
        {
            List<string> errors = validation.Errors
                .Select(failure => failure.PropertyName + ": " + failure.ErrorMessage)
                .ToList();

            return (HttpStatusCode.BadRequest, "One or more validation errors occurred.", errors);
        }

        if (exception is ArgumentException argument)
        {
            return (HttpStatusCode.BadRequest, argument.Message, new List<string>());
        }

        return (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.", new List<string>());
    }
}
