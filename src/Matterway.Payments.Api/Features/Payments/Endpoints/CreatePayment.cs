using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Matterway.Payments.Api.Features.Payments.Contracts;
using Matterway.Payments.Api.Features.Payments.Data;
using Matterway.Payments.Api.Features.Payments.Domain;

namespace Matterway.Payments.Api.Features.Payments.Endpoints;

public class CreatePayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Payments", Handler)
            .WithName("CreatePayment").WithSummary("Create a Payment.")
            .WithTags(nameof(Payment))
            .Produces<PaymentBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<PaymentBaseResponse>, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler([FromBody] PaymentBaseRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IPaymentRepository paymentRepository,
            IMapper mapper)
    {
        var newEntity = mapper.Map<Payment>(request);
        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

        if (!isValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        newEntity = await paymentRepository.Create(newEntity);
        if (newEntity is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = linkGenerator.GetPathByName(
            httpContext,
            "GetPaymentById",
            new { paymentId = newEntity.Id });

        return TypedResults.Created(location, mapper.Map<PaymentBaseResponse>(newEntity));
    }
}