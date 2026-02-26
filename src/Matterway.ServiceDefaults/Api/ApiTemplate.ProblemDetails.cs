using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.ServiceDefaults.Api;

public static partial class ApiTemplate
{
    private static void ApplyBadRequestProblemDetails(ProblemDetailsContext context)
    {
        if (context.Exception is not BadHttpRequestException
            {
                InnerException: JsonException jsonException
            })
        {
            if (context.Exception is BadHttpRequestException badHttpException)
            {
                context.HttpContext.Response.StatusCode = badHttpException.StatusCode;
                context.ProblemDetails = new ProblemDetails
                {
                    Status = badHttpException.StatusCode,
                    Title = "Bad Request",
                    Detail = badHttpException.Message,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Extensions =
                    {
                        ["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                    }
                };
            }

            return;
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var member = jsonException.Path?.TrimStart('$').TrimStart('.');
        if (!string.IsNullOrWhiteSpace(member))
            context.ProblemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [member] =
                [
                    $"Value provided for '{member}' has an invalid format. Please check the value and try again."
                ]
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid request payload format.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
        else
            context.ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid JSON payload.",
                Detail = "The request body contains malformed JSON. Please fix the payload and try again.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };

        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    }
}