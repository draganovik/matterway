using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Entities;
using Ordering.API.Models.OrderItemModels;
using Ordering.API.Repository;
using Shared.Enums;
using Shared.Models;
using Shared.ServiceBrokers;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Endpoints;

public static class OrderItemEndpoints
{
    public static void MapOrderItemEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Orders").WithTags(nameof(OrderItem));

        group.MapGet("/Items", QueryOrderItems)
            .WithName("QueryOrderItems").WithOpenApi();

        group.MapGet("/{id}/Items/{itemId}", GetOrderItemById)
            .WithName("GetOrderItemById").WithOpenApi();

        group.MapPut("/{id}/Items/{itemId}", UpdateOrderItemById)
            .WithName("UpdateOrderItemById").WithOpenApi();

        group.MapDelete("/{id}/Items/{itemId}", DeleteOrderItem)
            .WithName("DeleteOrderItem").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<OrderItemBaseResponseModel>>, NoContent>> QueryOrderItems([FromQuery] int pageIndex, [FromQuery] int pageSize, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        return await OrderItemRepository.Query(pageIndex, pageSize)
            is IEnumerable<OrderItem> entityCollection && entityCollection.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<OrderItemBaseResponseModel>>(entityCollection))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderItemBaseResponseModel>, NotFound>> GetOrderItemById(Guid id, Guid itemId, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        return await OrderItemRepository.GetById(id, itemId)
            is OrderItem entity
                ? TypedResults.Ok(mapper.Map<OrderItemBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderItemBaseResponseModel>, NotFound<object>, BadRequest<object>>> UpdateOrderItemById(Guid id, Guid itemId, OrderItemBaseRequestModel requestModel, IOrderItemRepository OrderItemRepository, ICatalogServiceBroker catalogServiceBroker, IMapper mapper)
    {
        var updatedEntity = mapper.Map<OrderItem>(requestModel);
        updatedEntity.OrderId = id;
        updatedEntity.ProductId = itemId;

        Product? product = await catalogServiceBroker.GetProductById(updatedEntity.ProductId);
        if (product is null)
        {
            return TypedResults.NotFound<object>(new { message = "Product not found" });
        }

        updatedEntity.ProductName = product.Title;
        updatedEntity.UnitPrice = product.Price ?? 0;

        var results = new List<ValidationResult>();
        var context = new ValidationContext(updatedEntity);
        var isValid = Validator.TryValidateObject(updatedEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updateEntity = await OrderItemRepository.Put(updatedEntity);
        if (updateEntity is null)
        {
            return TypedResults.NotFound<object>(new { message = "Cannot save entity" });
        }
        return TypedResults.Ok(mapper.Map<OrderItemBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrderItem(Guid id, Guid itemId, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        var isDeleted = await OrderItemRepository.Delete(id, itemId);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
