using AutoMapper;
using Matterway.Ordering.Api.Features.Orders.Contracts;
using Matterway.Ordering.Api.Features.Orders.Domain;

namespace Matterway.Ordering.Api.Features.Orders.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderBaseResponse>()
            .ForMember(dest => dest.Total,
                opt => opt.MapFrom(src => Math.Round(src.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity), 2)));

        CreateMap<OrderCreateRequest, Order>();
        CreateMap<OrderUpdateRequest, Order>();
    }
}