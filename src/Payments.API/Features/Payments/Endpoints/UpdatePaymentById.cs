using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Features.Payments.Contracts;
using Payments.API.Features.Payments.Data;
using Payments.API.Features.Payments.Domain;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Payments.API.Features.Payments.Endpoints;

public class UpdatePaymentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Payments/{id:guid}", Handler)
            .WithName("UpdatePaymentById").WithSummary("Update Payment by id.")
            .WithTags(nameof(Payment))
            .Produces<PaymentBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(nameof(SystemUserRole.Admin)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaymentBaseResponse>, NotFound, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler(Guid id,
            PaymentBaseRequest request,
            IPaymentRepository paymentRepository,
            IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);
        var isValid = Validator.TryValidateObject(request, context, results, true);

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

        var updateEntity = await paymentRepository.Update(id, request);
        return updateEntity is not null
            ? TypedResults.Ok(mapper.Map<PaymentBaseResponse>(updateEntity))
            : TypedResults.NotFound();
    }
}