using AutoMapper;
using Ordering.API.Entities;
using Ordering.API.Models.OrderModels;

namespace Ordering.API.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderBaseResponseModel>();

        CreateMap<OrderCreateRequestModel, Order>();
        CreateMap<OrderUpdateRequestModel, Order>();
    }
}
