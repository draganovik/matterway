using AutoMapper;
using Ordering.API.Entities;
using Ordering.API.Models.OrderHistoryModels;

namespace Ordering.API.Profiles;

public class OrderHistoryProfile : Profile
{
    public OrderHistoryProfile()
    {
        CreateMap<OrderHistory, OrderHistoryBaseResponseModel>();
        CreateMap<OrderHistory, OrderHistoryOrderResponseModel>();

        CreateMap<OrderHistoryBaseRequestModel, OrderHistory>();
    }
}