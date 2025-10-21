using AutoMapper;
using Ordering.API.Features.OrderItems.Contracts;
using Ordering.API.Features.OrderItems.Domain;

namespace Ordering.API.Features.OrderItems.Mapping;

public class OrderItemProfile : Profile
{
    public OrderItemProfile()
    {
        CreateMap<OrderItem, OrderItemBaseResponse>();
        CreateMap<OrderItem, OrderItemOrderResponse>();

        CreateMap<OrderItemBaseRequest, OrderItem>();
    }
}