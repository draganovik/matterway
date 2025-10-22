using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Customers.API.Features.CartItems.Contracts;
using Customers.API.Features.CartItems.Data;
using Customers.API.Features.CartItems.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using Shared.ServiceBrokers;

namespace Customers.API.Features.CartItems.Endpoints;

public class PutCartItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Customers/{id:guid}/CartItems/{productId:guid}", Handler)
            .WithName("UpdateCartItemById").WithSummary("Upsert CartItem.")
            .WithTags(nameof(CartItem))
            .Produces<CartItemBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<CartItemBaseResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult,
                ValidationProblem>>
        Handler(Guid id,
            Guid productId,
            CartItemBaseRequest request,
            HttpContext httpContext,
            ICartItemRepository cartItemRepository,
            ICatalogServiceBroker catalogServiceBroker,
            IMapper mapper)
    {
        var newEntity = mapper.Map<CartItem>(request);
        newEntity.CustomerId = id;
        newEntity.ProductId = productId;

        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();

        if (userRole == SystemUserRole.Customer && id != systemUserId) return TypedResults.Forbid();

        var product = await catalogServiceBroker.GetProductById(newEntity.ProductId);
        if (product is null) return TypedResults.NotFound();

        newEntity.ProductName = product.Title;
        newEntity.UnitPrice = product.Price ?? 0;

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
            return TypedResults.BadRequest(problemDetails);
        }

        return newEntity is not null
            ? TypedResults.Ok(mapper.Map<CartItemBaseResponse>(newEntity))
            : TypedResults.NotFound();
    }
}