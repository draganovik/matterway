using AutoMapper;
using Ordering.API.Features.OrderHistories.Contracts;
using Ordering.API.Features.OrderHistories.Domain;

namespace Ordering.API.Features.OrderHistories.Mapping;

public class OrderHistoryProfile : Profile
{
    public OrderHistoryProfile()
    {
        CreateMap<OrderHistory, OrderHistoryBaseResponse>();
        CreateMap<OrderHistory, OrderHistoryOrderResponse>();

        CreateMap<OrderHistoryBaseRequest, OrderHistory>();
    }
}