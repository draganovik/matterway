using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Ordering.API.Entities;
using Ordering.API.Models.OrderModels;
using Ordering.API.Repository;
using Shared.Enums;
using Shared.ServiceBrokers;
using SharedProject.ModelTemplates;

namespace Ordering.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Orders").WithTags(nameof(Order));

        group.MapGet("/", QueryOrders)
            .WithName("QueryOrders").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Query Orders"
            });

        group.MapGet("/{id}", GetOrderById)
            .WithName("GetOrderById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get Order By Id"
            });

        group.MapPatch("/{id}", UpdateOrderById)
            .WithName("UpdateOrderById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update Order By Id"
            });

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Create Order"
            });

        group.MapDelete("/{id}", DeleteOrder)
            .WithName("DeleteOrder").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Delete Order"
            });
    }


    [Authorize]
    public static async
        Task<Results<Ok<PaginationResponse<OrderBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>,
            ForbidHttpResult>> QueryOrders([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext,
            IOrderRepository orderRepository, IMapper mapper)
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

        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();
        int total;
        IEnumerable<Order>? entities;
        if (userRole == SystemUserRole.Customer)
        {
            total = await orderRepository.GetTotalEntities(systemUserId);
            entities = await orderRepository.QueryByCustomerId(systemUserId, page, pageSize);
        }
        else
        {
            total = await orderRepository.GetTotalEntities();
            entities = await orderRepository.Query(page, pageSize);
        }

        var baseUri =
            new Uri(
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Orders");
        var paginationResponse = new PaginationResponse<OrderBaseResponseModel>(total, page, pageSize,
            mapper.Map<IEnumerable<OrderBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Order> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<OrderBaseResponseModel>, NotFound>> GetOrderById(Guid id,
        IOrderRepository orderRepository, IMapper mapper)
    {
        return await orderRepository.GetById(id)
            is Order entity
            ? TypedResults.Ok(mapper.Map<OrderBaseResponseModel>(entity))
            : TypedResults.NotFound();
    }

    [Authorize]
    public static async
        Task<Results<Ok<OrderBaseResponseModel>, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult,
            ForbidHttpResult>> UpdateOrderById(Guid id, OrderUpdateRequestModel requestModel, HttpContext httpContext,
            IOrderRepository orderRepository, ICustomersServiceBroker customerServiceBroker, IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Unauthorized();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Unauthorized();

        if (requestModel.CustomerId != null && userRole == SystemUserRole.Customer)
        {
            var customerId = await customerServiceBroker.VerifyBySystemUserId(systemUserId);
            if (customerId == null || customerId != requestModel.CustomerId.Value) return TypedResults.Forbid();
        }
        else if (requestModel.CustomerId != null &&
                 !await customerServiceBroker.VerifyByCustomerId(requestModel.CustomerId.Value))
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer Id is not registrated in Customers API"
            };
            return TypedResults.BadRequest(problemDetails);
        }

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

        var updateEntity = await orderRepository.Update(id, requestModel);
        if (updateEntity is null) return TypedResults.NotFound();
        return TypedResults.Ok(mapper.Map<OrderBaseResponseModel>(updateEntity));
    }

    public static async
        Task<Results<Created<OrderBaseResponseModel>, BadRequest<ProblemDetails>, UnauthorizedHttpResult,
            ForbidHttpResult>> CreateOrder(OrderCreateRequestModel requestModel, HttpContext httpContext,
            IOrderRepository orderRepository, ICustomersServiceBroker customerServiceBroker, IMapper mapper,
            IConfiguration configuration)
    {
        /*var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Unauthorized();
        }

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            return TypedResults.Unauthorized();
        }

        var newEntity = mapper.Map<Order>(requestModel);

        if (newEntity.CustomerId != null && userRole == SystemUserRole.Customer)
        {
            var customerId = await customerServiceBroker.VerifyBySystemUserId(systemUserId);
            if (customerId == null || customerId != newEntity.CustomerId.Value)
            {
                return TypedResults.Forbid();
            }
        }
        else if (newEntity.CustomerId != null && !await customerServiceBroker.VerifyByCustomerId(newEntity.CustomerId.Value))
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer Id is not registrated in Customers API"
            };
            return TypedResults.BadRequest(problemDetails);
        }*/
        var newEntity = mapper.Map<Order>(requestModel);
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

        newEntity = await orderRepository.Create(newEntity);
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

        return TypedResults.Created($"/api/Orders/{newEntity.Id}", mapper.Map<OrderBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteOrder(Guid id, IOrderRepository orderRepository,
        IMapper mapper)
    {
        var isDeleted = await orderRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}