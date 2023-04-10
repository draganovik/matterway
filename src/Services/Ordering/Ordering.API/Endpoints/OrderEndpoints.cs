using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.API.Entities;
using Ordering.API.Models.OrderModels;
using Ordering.API.Repository;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Orders").WithTags(nameof(Order));

        group.MapGet("/", QueryOrders)
            .WithName("QueryOrders").WithOpenApi();

        group.MapGet("/{id}", GetOrderById)
            .WithName("GetOrderById").WithOpenApi();

        group.MapPut("/{id}", UpdateOrderById)
            .WithName("UpdateOrderById").WithOpenApi();

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder").WithOpenApi();

        group.MapDelete("/{id}", DeleteOrder)
            .WithName("DeleteOrder").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<OrderBaseResponseModel>>, NoContent>> QueryOrders(IOrderRepository OrderRepository, IMapper mapper)
    {
        return await OrderRepository.Query()
            is IEnumerable<Order> entityCollection && entityCollection.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<OrderBaseResponseModel>>(entityCollection))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderBaseResponseModel>, NotFound>> GetOrderById(Guid id, IOrderRepository OrderRepository, IMapper mapper)
    {
        return await OrderRepository.GetById(id)
            is Order entity
                ? TypedResults.Ok(mapper.Map<OrderBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderBaseResponseModel>, NotFound<object>, BadRequest<object>>> UpdateOrderById(Guid id, OrderUpdateRequestModel requestModel, IOrderRepository OrderRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updateEntity = await OrderRepository.Update(id, requestModel);
        if (updateEntity is null)
        {
            return TypedResults.NotFound<object>(new { message = "Entity not found" });
        }
        return TypedResults.Ok(mapper.Map<OrderBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<OrderBaseResponseModel>, BadRequest<object>>> CreateOrder(OrderCreateRequestModel requestModel, IOrderRepository OrderRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<Order>(requestModel);
        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        newEntity = await OrderRepository.Create(newEntity);
        if (newEntity is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create object" });
        }
        return TypedResults.Created($"/api/Orders/{newEntity.Id}", mapper.Map<OrderBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrder(Guid id, IOrderRepository OrderRepository, IMapper mapper)
    {
        var isDeleted = await OrderRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
