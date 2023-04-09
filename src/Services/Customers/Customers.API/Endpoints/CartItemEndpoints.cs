using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CartItemModels;
using Customers.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;

namespace Customers.API.Endpoints;

public static class CartItemEndpoints
{
    public static void MapCartItemEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/CartItems").WithTags(nameof(CartItem));

        group.MapGet("/", QueryCartItems)
            .WithName("QueryCartItems").WithOpenApi();

        group.MapGet("/{id}", GetCartItemById)
            .WithName("GetCartItemById").WithOpenApi();

        group.MapPut("/{id}", UpdateCartItemById)
            .WithName("UpdateCartItemById").WithOpenApi();

        group.MapPost("/", CreateCartItem)
            .WithName("CreateCartItem").WithOpenApi();

        group.MapDelete("/{id}", DeleteCartItem)
            .WithName("DeleteCartItem").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<CartItemBaseResponseModel>>, NoContent>> QueryCartItems(ICartItemRepository cartItemRepository, IMapper mapper)
    {
        return await cartItemRepository.Query()
            is IEnumerable<CartItem> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<CartItemBaseResponseModel>>(value))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<CartItemBaseResponseModel>, NotFound>> GetCartItemById(Guid id, ICartItemRepository cartItemRepository, IMapper mapper)
    {
        return await cartItemRepository.GetById(id)
            is CartItem value
                ? TypedResults.Ok(mapper.Map<CartItemBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<CartItemBaseResponseModel>, NotFound>> UpdateCartItemById(Guid id, CartItemUpdateRequestModel requestModel, ICartItemRepository cartItemRepository, IMapper mapper)
    {
        var updatedUser = await cartItemRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<CartItemBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<CartItemBaseResponseModel>, BadRequest>> CreateCartItem(CartItemCreateRequestModel requestModel, ICartItemRepository cartItemRepository, IMapper mapper)
    {
        var cartItemModel = mapper.Map<CartItem>(requestModel);
        var createdCartItem = await cartItemRepository.Create(cartItemModel);
        if (createdCartItem is null)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Created($"/api/CartItems/{createdCartItem.Id}", mapper.Map<CartItemBaseResponseModel>(createdCartItem));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteCartItem(Guid id, ICartItemRepository cartItemRepository, IMapper mapper)
    {
        var isDeleted = await cartItemRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
