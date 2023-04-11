using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CartItemModels;

namespace Customers.API.Profiles;

public class CartItemProfile : Profile
{
    public CartItemProfile()
    {
        CreateMap<CartItem, CartItemBaseResponseModel>();

        CreateMap<CartItemBaseRequestModel, CartItem>();
    }
}
