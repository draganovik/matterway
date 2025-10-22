using AutoMapper;
using Customers.Api.Features.CartItems.Contracts;
using Customers.Api.Features.CartItems.Domain;

namespace Customers.Api.Features.CartItems.Mapping;

public class CartItemProfile : Profile
{
    public CartItemProfile()
    {
        CreateMap<CartItem, CartItemBaseResponse>();
        CreateMap<CartItemBaseRequest, CartItem>();
    }
}