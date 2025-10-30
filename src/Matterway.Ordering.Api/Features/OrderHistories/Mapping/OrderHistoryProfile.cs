using AutoMapper;
using Matterway.Ordering.Api.Features.OrderHistories.Contracts;
using Matterway.Ordering.Api.Features.OrderHistories.Domain;

namespace Matterway.Ordering.Api.Features.OrderHistories.Mapping;

public class OrderHistoryProfile : Profile
{
    public OrderHistoryProfile()
    {
        CreateMap<OrderHistory, OrderHistoryBaseResponse>();
        CreateMap<OrderHistory, OrderHistoryOrderResponse>();

        CreateMap<OrderHistoryBaseRequest, OrderHistory>();
    }
}