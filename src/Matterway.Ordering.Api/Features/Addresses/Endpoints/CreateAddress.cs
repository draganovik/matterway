using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Application;
using Matterway.Ordering.Api.Features.Addresses.Contracts;
using Matterway.Ordering.Api.Features.Addresses.Data;
using Matterway.Ordering.Api.Features.Addresses.Domain;

namespace Matterway.Ordering.Api.Features.Addresses.Endpoints;

public class CreateAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Addresses", Handler)
            .WithName("CreateAddress").WithSummary("Create an Address.")
            .WithTags(nameof(Address))
            .Produces<AddressBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddressBaseResponse>, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler(AddressBaseRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IAddressRepository addressRepository,
            IMapper mapper)
    {
        var newEntity = mapper.Map<Address>(request);

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

        newEntity = await addressRepository.Create(newEntity);
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

        var location = linkGenerator.GetPathByName(httpContext, "GetAddressById", new { id = newEntity.Id });

        return TypedResults.Created(location, mapper.Map<AddressBaseResponse>(newEntity));
    }
}