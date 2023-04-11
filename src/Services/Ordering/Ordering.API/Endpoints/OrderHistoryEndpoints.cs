using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Entities;
using Ordering.API.Models.OrderHistoryModels;
using Ordering.API.Repository;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Endpoints;

public static class OrderHistoryEndpoints
{
    public static void MapOrderHistoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OrderHistories").WithTags(nameof(OrderHistory));

        group.MapGet("/", QueryOrderHistories)
            .WithName("QueryOrderHistories").WithOpenApi();

        group.MapGet("/{id}", GetOrderHistoryById)
            .WithName("GetOrderHistoryById").WithOpenApi();

        group.MapPut("/{id}", UpdateOrderHistoryById)
            .WithName("UpdateOrderHistoryById").WithOpenApi();

        group.MapPost("/", CreateOrderHistory)
            .WithName("CreateOrderHistory").WithOpenApi();

        group.MapDelete("/{id}", DeleteOrderHistory)
            .WithName("DeleteOrderHistory").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<OrderHistoryBaseResponseModel>>, NoContent>> QueryOrderHistories([FromQuery] int pageIndex, [FromQuery] int pageSize, IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        return await orderHistoryRepository.Query(pageIndex, pageSize)
            is IEnumerable<OrderHistory> entityCollection && entityCollection.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<OrderHistoryBaseResponseModel>>(entityCollection))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderHistoryBaseResponseModel>, NotFound>> GetOrderHistoryById(Guid id, IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        return await orderHistoryRepository.GetById(id)
            is OrderHistory entity
                ? TypedResults.Ok(mapper.Map<OrderHistoryBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderHistoryBaseResponseModel>, NotFound<object>, BadRequest<object>>> UpdateOrderHistoryById(Guid id, OrderHistoryBaseRequestModel requestModel, IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updateEntity = await orderHistoryRepository.Update(id, requestModel);
        if (updateEntity is null)
        {
            return TypedResults.NotFound<object>(new { message = "Entity not found" });
        }
        return TypedResults.Ok(mapper.Map<OrderHistoryBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<OrderHistoryBaseResponseModel>, BadRequest<object>>> CreateOrderHistory(OrderHistoryBaseRequestModel requestModel, IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<OrderHistory>(requestModel);
        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        newEntity = await orderHistoryRepository.Create(newEntity);
        if (newEntity is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create entity" });
        }
        return TypedResults.Created($"/api/OrderHistories/{newEntity.Id}", mapper.Map<OrderHistoryBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrderHistory(Guid id, IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        var isDeleted = await orderHistoryRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
