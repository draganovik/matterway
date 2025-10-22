using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Ordering.API.Features.Addresses.Contracts;
using Ordering.API.Features.Addresses.Data;
using Ordering.API.Features.Addresses.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Ordering.API.Features.Addresses.Endpoints;

public class UpdateAddressById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Addresses/{id:guid}", Handler)
            .WithName("UpdateAddressById").WithSummary("Update Address by id.")
            .WithTags(nameof(Address))
            .Produces<AddressBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<AddressBaseResponse>, NotFound, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler(Guid id,
            AddressBaseRequest request,
            IAddressRepository addressRepository,
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

        var updateEntity = await addressRepository.Update(id, request);
        return updateEntity is not null
            ? TypedResults.Ok(mapper.Map<AddressBaseResponse>(updateEntity))
            : TypedResults.NotFound();
    }
}