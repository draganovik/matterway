using AutoMapper;
using Matterway.Ordering.Api.Features.Addresses.Contracts;
using Matterway.Ordering.Api.Features.Addresses.Domain;

namespace Matterway.Ordering.Api.Features.Addresses.Mapping;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressBaseResponse>();
        CreateMap<Address, AddressOrderResponse>();

        CreateMap<AddressBaseRequest, Address>();
    }
}