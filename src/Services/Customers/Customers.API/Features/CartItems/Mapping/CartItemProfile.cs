using AutoMapper;
using Customers.API.Features.CartItems.Contracts;
using Customers.API.Features.CartItems.Domain;

namespace Customers.API.Features.CartItems.Mapping;

public class CartItemProfile : Profile
{
    public CartItemProfile()
    {
        CreateMap<CartItem, CartItemBaseResponse>();
        CreateMap<CartItemBaseRequest, CartItem>();
    }
}