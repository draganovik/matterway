using AutoMapper;
using Matterway.Customers.Api.Features.CartItems.Contracts;
using Matterway.Customers.Api.Features.CartItems.Domain;

namespace Matterway.Customers.Api.Features.CartItems.Mapping;

public class CartItemProfile : Profile
{
    public CartItemProfile()
    {
        CreateMap<CartItem, CartItemBaseResponse>();
        CreateMap<CartItemBaseRequest, CartItem>();
    }
}