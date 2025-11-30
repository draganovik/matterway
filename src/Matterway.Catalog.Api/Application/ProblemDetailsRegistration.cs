using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Application;

public static class ProblemDetailsRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureProblemDetails()
        {
            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    if (ctx.Exception is not BadHttpRequestException
                        {
                            InnerException: JsonException jsonException
                        })
                    {
                        if (ctx.Exception is BadHttpRequestException badHttpException)
                        {
                            ctx.HttpContext.Response.StatusCode = badHttpException.StatusCode;
                            ctx.ProblemDetails = new ProblemDetails
                            {
                                Status = badHttpException.StatusCode,
                                Title = "Bad Request",
                                Detail = badHttpException.Message,
                                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                            };
                            ctx.ProblemDetails.Extensions["traceId"] =
                                Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
                        }

                        return;
                    }

                    ctx.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var member = jsonException.Path?.TrimStart('$').TrimStart('.');
                    if (!string.IsNullOrWhiteSpace(member))
                        ctx.ProblemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>
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
                        ctx.ProblemDetails = new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Invalid JSON payload.",
                            Detail = "The request body contains malformed JSON. Please fix the payload and try again.",
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                        };

                    ctx.ProblemDetails.Extensions["traceId"] =
                        Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
                };
            });

            return builder;
        }
    }
}