using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CartItemModels;
using Customers.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models;
using Shared.ServiceBrokers;
using SharedProject.ModelTemplates;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Customers.API.Endpoints;

public static class CartItemEndpoints
{
    public static void MapCartItemEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Customers").WithTags(nameof(CartItem));

        group.MapGet("/CartItems", QueryCartItems)
            .WithName("QueryCartItems").WithOpenApi(operation => new(operation)
            {
                Summary = "Query CartItems",
            });

        group.MapGet("/{id}/CartItems/{productId}", GetCartItemById)
            .WithName("GetCartItemById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get CartItem by Id",
            });

        group.MapPut("/{id}/CartItems/{productId}", PutCartItem)
            .WithName("UpdateCartItemById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update CartItem by Id",
            });

        group.MapDelete("{id}/CartItems/{productId}", DeleteCartItem)
            .WithName("DeleteCartItem").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete CartItem",
            });
    }


    [Authorize]
    public static async Task<Results<Ok<PaginationResponse<CartItemBaseResponseModel>>, NoContent, ForbidHttpResult, BadRequest<ProblemDetails>>> QueryCartItems([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext, ICartItemRepository cartItemRepository, IMapper mapper)
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
            if (page < 1)
            {
                results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            }
            if (pageSize < 1)
            {
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));
            }

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Forbid();
        }

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            return TypedResults.Forbid();
        }
        int total;
        IEnumerable<CartItem>? entities;

        if (userRole == SystemUserRole.Customer)
        {
            total = await cartItemRepository.GetTotalEntities(systemUserId);
            entities = await cartItemRepository.QueryByCustomerId(systemUserId, page, pageSize);
        }
        else
        {
            total = await cartItemRepository.GetTotalEntities();
            entities = await cartItemRepository.Query(page, pageSize);
        }

        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Customers/CartItems");
        var paginationResponse = new PaginationResponse<CartItemBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<CartItemBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<CartItem> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<CartItemBaseResponseModel>, NotFound>> GetCartItemById(Guid id, Guid productId, ICartItemRepository cartItemRepository, IMapper mapper)
    {
        return await cartItemRepository.GetById(id, productId)
            is CartItem value
                ? TypedResults.Ok(mapper.Map<CartItemBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize]
    public static async Task<Results<Ok<CartItemBaseResponseModel>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>> PutCartItem(Guid id, Guid productId, CartItemBaseRequestModel requestModel, HttpContext httpContext, ICartItemRepository cartItemRepository, ICatalogServiceBroker catalogServiceBroker, IMapper mapper)
    {
        var newEntity = mapper.Map<CartItem>(requestModel);
        newEntity.CustomerId = id;
        newEntity.ProductId = productId;

        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Forbid();
        }

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            return TypedResults.Forbid();
        }

        if (userRole == SystemUserRole.Customer && id != systemUserId)
        {
            return TypedResults.Forbid();
        }

        Product? product = await catalogServiceBroker.GetProductById(newEntity.ProductId);
        if (product is null)
        {
            return TypedResults.NotFound();
        }

        newEntity.ProductName = product.Title;
        newEntity.UnitPrice = product.Price ?? 0;

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
        try
        {
            newEntity = await cartItemRepository.Put(newEntity);
        }
        catch (Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            };
            return TypedResults.BadRequest<ProblemDetails>(problemDetails);
        }
        return newEntity is not null ? TypedResults.Ok(mapper.Map<CartItemBaseResponseModel>(newEntity)) : TypedResults.NotFound();
    }

    [Authorize]
    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteCartItem(Guid id, Guid productId, HttpContext httpContext, ICartItemRepository cartItemRepository, IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Forbid();
        }

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            return TypedResults.Forbid();
        }

        if (userRole != SystemUserRole.Admin)
        {
            if (id != systemUserId)
            {
                return TypedResults.Forbid();
            }
        }

        var isDeleted = await cartItemRepository.Delete(id, productId);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
