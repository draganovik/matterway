using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.API.Entities;
using Ordering.API.Models.OrderItemModels;
using Ordering.API.Repository;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Endpoints;

public static class OrderItemEndpoints
{
    public static void MapOrderItemEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OrderItems").WithTags(nameof(OrderItem));

        group.MapGet("/", QueryOrderItems)
            .WithName("QueryOrderItems").WithOpenApi();

        group.MapGet("/{id}", GetOrderItemById)
            .WithName("GetOrderItemById").WithOpenApi();

        group.MapPut("/{id}", UpdateOrderItemById)
            .WithName("UpdateOrderItemById").WithOpenApi();

        group.MapPost("/", CreateOrderItem)
            .WithName("CreateOrderItem").WithOpenApi();

        group.MapDelete("/{id}", DeleteOrderItem)
            .WithName("DeleteOrderItem").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<OrderItemBaseResponseModel>>, NoContent>> QueryOrderItems(IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        return await OrderItemRepository.Query()
            is IEnumerable<OrderItem> entityCollection && entityCollection.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<OrderItemBaseResponseModel>>(entityCollection))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderItemBaseResponseModel>, NotFound>> GetOrderItemById(Guid id, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        return await OrderItemRepository.GetById(id)
            is OrderItem entity
                ? TypedResults.Ok(mapper.Map<OrderItemBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderItemBaseResponseModel>, NotFound<object>, BadRequest<object>>> UpdateOrderItemById(Guid id, OrderItemBaseRequestModel requestModel, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updatedEntity = mapper.Map<OrderItem>(requestModel);

        var updateEntity = await OrderItemRepository.Update(id, updatedEntity);
        if (updateEntity is null)
        {
            return TypedResults.NotFound<object>(new { message = "Entity not found" });
        }
        return TypedResults.Ok(mapper.Map<OrderItemBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<OrderItemBaseResponseModel>, BadRequest<object>>> CreateOrderItem(OrderItemBaseRequestModel requestModel, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<OrderItem>(requestModel);
        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        newEntity = await OrderItemRepository.Create(newEntity);
        if (newEntity is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create object" });
        }
        return TypedResults.Created($"/api/OrderItems/{newEntity.Id}", mapper.Map<OrderItemBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrderItem(Guid id, IOrderItemRepository OrderItemRepository, IMapper mapper)
    {
        var isDeleted = await OrderItemRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
