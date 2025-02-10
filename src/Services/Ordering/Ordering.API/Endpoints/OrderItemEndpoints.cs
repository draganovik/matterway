using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Ordering.API.Entities;
using Ordering.API.Models.OrderItemModels;
using Ordering.API.Repository;
using Shared.Enums;
using Shared.ServiceBrokers;
using SharedProject.ModelTemplates;

namespace Ordering.API.Endpoints;

public static class OrderItemEndpoints
{
    public static void MapOrderItemEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Orders").WithTags(nameof(OrderItem));

        group.MapGet("/Items", QueryOrderItems)
            .WithName("QueryOrderItems").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Query Order Items"
            });

        group.MapGet("/{id}/Items/{itemId}", GetOrderItemById)
            .WithName("GetOrderItemById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get Order Item By Id"
            });

        group.MapPut("/{id}/Items/{itemId}", UpdateOrderItemById)
            .WithName("UpdateOrderItemById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update Order Item By Id"
            });

        group.MapDelete("/{id}/Items/{itemId}", DeleteOrderItem)
            .WithName("DeleteOrderItem").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Delete Order Item"
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async
        Task<Results<Ok<PaginationResponse<OrderItemBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>>
        QueryOrderItems([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext,
            IOrderItemRepository orderItemRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero."
            };
            var results = new List<ValidationResult>();
            if (page < 1) results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            if (pageSize < 1)
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await orderItemRepository.GetTotalEntities();
        var entities = await orderItemRepository.Query(page, pageSize);
        var baseUri =
            new Uri(
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Orders/Items");
        var paginationResponse = new PaginationResponse<OrderItemBaseResponseModel>(total, page, pageSize,
            mapper.Map<IEnumerable<OrderItemBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<OrderItem> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderItemBaseResponseModel>, NotFound>> GetOrderItemById(Guid id, Guid itemId,
        IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        return await OrderItemRepository.GetById(id, itemId)
            is OrderItem entity
            ? TypedResults.Ok(mapper.Map<OrderItemBaseResponseModel>(entity))
            : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<OrderItemBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>>
        UpdateOrderItemById(Guid id, Guid itemId, OrderItemBaseRequestModel requestModel,
            IOrderItemRepository OrderItemRepository, ICatalogServiceBroker catalogServiceBroker, IMapper mapper)
    {
        var updatedEntity = mapper.Map<OrderItem>(requestModel);
        updatedEntity.OrderId = id;
        updatedEntity.ProductId = itemId;

        var product = await catalogServiceBroker.GetProductById(updatedEntity.ProductId);
        if (product is null) return TypedResults.NotFound();

        updatedEntity.ProductName = product.Title;
        updatedEntity.UnitPrice = product.Price ?? 0;

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

        var updateEntity = await OrderItemRepository.Put(updatedEntity);
        if (updateEntity is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Ok(mapper.Map<OrderItemBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrderItem(Guid id, Guid itemId,
        IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        var isDeleted = await OrderItemRepository.Delete(id, itemId);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}