using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Ordering.API.Entities;
using Ordering.API.Models.OrderHistoryModels;
using Ordering.API.Repository;
using Shared.Enums;
using SharedProject.ModelTemplates;

namespace Ordering.API.Endpoints;

public static class OrderHistoryEndpoints
{
    public static void MapOrderHistoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/OrderHistories").WithTags(nameof(OrderHistory));

        group.MapGet("/", QueryOrderHistories)
            .WithName("QueryOrderHistories").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Query Order Histories"
            });

        group.MapGet("/{id}", GetOrderHistoryById)
            .WithName("GetOrderHistoryById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get Order History By Id"
            });

        group.MapPatch("/{id}", UpdateOrderHistoryById)
            .WithName("UpdateOrderHistoryById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update Order History By Id"
            });

        group.MapPost("/", CreateOrderHistory)
            .WithName("CreateOrderHistory").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Create Order History"
            });

        group.MapDelete("/{id}", DeleteOrderHistory)
            .WithName("DeleteOrderHistory").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Delete Order History"
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async
        Task<Results<Ok<PaginationResponse<OrderHistoryBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>>
        QueryOrderHistories([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext,
            IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
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

        var total = await orderHistoryRepository.GetTotalEntities();
        var entities = await orderHistoryRepository.Query(page, pageSize);
        var baseUri =
            new Uri(
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/OrderHistories");
        var paginationResponse = new PaginationResponse<OrderHistoryBaseResponseModel>(total, page, pageSize,
            mapper.Map<IEnumerable<OrderHistoryBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<OrderHistory> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderHistoryBaseResponseModel>, NotFound>> GetOrderHistoryById(Guid id,
        IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        return await orderHistoryRepository.GetById(id)
            is OrderHistory entity
            ? TypedResults.Ok(mapper.Map<OrderHistoryBaseResponseModel>(entity))
            : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderHistoryBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>>
        UpdateOrderHistoryById(Guid id, OrderHistoryBaseRequestModel requestModel,
            IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

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

        var updateEntity = await orderHistoryRepository.Update(id, requestModel);
        if (updateEntity is null) return TypedResults.NotFound();
        return TypedResults.Ok(mapper.Map<OrderHistoryBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<OrderHistoryBaseResponseModel>, BadRequest<ProblemDetails>>>
        CreateOrderHistory(OrderHistoryBaseRequestModel requestModel, IOrderHistoryRepository orderHistoryRepository,
            IMapper mapper)
    {
        var newEntity = mapper.Map<OrderHistory>(requestModel);
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

        newEntity = await orderHistoryRepository.Create(newEntity);
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

        return TypedResults.Created($"/api/OrderHistories/{newEntity.Id}",
            mapper.Map<OrderHistoryBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrderHistory(Guid id,
        IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        var isDeleted = await orderHistoryRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}