using AutoMapper;
using Ordering.Api.Features.OrderHistories.Contracts;
using Ordering.Api.Features.OrderHistories.Domain;

namespace Ordering.Api.Features.OrderHistories.Mapping;

public class OrderHistoryProfile : Profile
{
    public OrderHistoryProfile()
    {
        CreateMap<OrderHistory, OrderHistoryBaseResponse>();
        CreateMap<OrderHistory, OrderHistoryOrderResponse>();

        CreateMap<OrderHistoryBaseRequest, OrderHistory>();
    }
}