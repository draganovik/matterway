using AutoMapper;
using Matterway.Ordering.Api.Features.OrderItems.Contracts;
using Matterway.Ordering.Api.Features.OrderItems.Domain;

namespace Matterway.Ordering.Api.Features.OrderItems.Mapping;

public class OrderItemProfile : Profile
{
    public OrderItemProfile()
    {
        CreateMap<OrderItem, OrderItemBaseResponse>();
        CreateMap<OrderItem, OrderItemOrderResponse>();

        CreateMap<OrderItemBaseRequest, OrderItem>();
    }
}