using AutoMapper;
using Ordering.API.Entities;
using Ordering.API.Models.OrderItemModels;

namespace Ordering.API.Profiles;

public class OrderItemProfile : Profile
{
    public OrderItemProfile()
    {
        CreateMap<OrderItem, OrderItemBaseResponseModel>();
        CreateMap<OrderItem, OrderItemOrderResponseModel>();

        CreateMap<OrderItemBaseRequestModel, OrderItem>();
    }
}