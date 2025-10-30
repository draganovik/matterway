using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Matterway.Ordering.Api.Features.OrderHistories.Contracts;
using Matterway.Ordering.Api.Features.OrderHistories.Data;
using Matterway.Ordering.Api.Features.OrderHistories.Domain;

namespace Matterway.Ordering.Api.Features.OrderHistories.Endpoints;

public class UpdateOrderHistoryById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("OrderHistories/{id:guid}", Handler)
            .WithName("UpdateOrderHistoryById").WithSummary("Update Order History by id.")
            .WithTags(nameof(OrderHistory))
            .Produces<OrderHistoryBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
        Results<Ok<OrderHistoryBaseResponse>, NotFound, BadRequest<ProblemDetails>, ValidationProblem>> Handler(
        Guid id,
        OrderHistoryBaseRequest request,
        IOrderHistoryRepository orderHistoryRepository,
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

        var updateEntity = await orderHistoryRepository.Update(id, request);
        return updateEntity is not null
            ? TypedResults.Ok(mapper.Map<OrderHistoryBaseResponse>(updateEntity))
            : TypedResults.NotFound();
    }
}