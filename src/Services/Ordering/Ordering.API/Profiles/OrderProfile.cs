using AutoMapper;
using Ordering.API.Entities;
using Ordering.API.Models.OrderModels;

namespace Ordering.API.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderBaseResponseModel>()
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => Math.Round(src.OrderItems.Sum(oi => oi.UnitPrice * oi.Units), 2)));

        CreateMap<OrderCreateRequestModel, Order>();
        CreateMap<OrderUpdateRequestModel, Order>();
    }
}
