using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Services.Brokers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Application;
using Matterway.Ordering.Api.Features.OrderItems.Contracts;
using Matterway.Ordering.Api.Features.OrderItems.Data;
using Matterway.Ordering.Api.Features.OrderItems.Domain;

namespace Matterway.Ordering.Api.Features.OrderItems.Endpoints;

public class PutOrderItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Orders/{id:guid}/Items/{itemId:guid}", Handler)
            .WithName("UpdateOrderItemById").WithSummary("Create or update an Order Item.")
            .WithTags(nameof(OrderItem))
            .Produces<OrderItemBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<OrderItemBaseResponse>, NotFound, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler(Guid id,
            Guid itemId,
            OrderItemBaseRequest request,
            IOrderItemRepository orderItemRepository,
            ICatalogServiceBroker catalogServiceBroker,
            IMapper mapper)
    {
        var updatedEntity = mapper.Map<OrderItem>(request);
        updatedEntity.OrderId = id;
        updatedEntity.ProductId = itemId;

        var product = await catalogServiceBroker.GetProductById(updatedEntity.ProductId);
        if (product is null) return TypedResults.NotFound();

        updatedEntity.ProductName = product.Title;
        updatedEntity.UnitPrice = product.Price ?? product.BasePrice ?? 0;

        var results = new List<ValidationResult>();
        var context = new ValidationContext(updatedEntity);
        var isValid = Validator.TryValidateObject(updatedEntity, context, results, true);

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

        var updated = await orderItemRepository.Put(updatedEntity);
        if (updated is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Ok(mapper.Map<OrderItemBaseResponse>(updated));
    }
}