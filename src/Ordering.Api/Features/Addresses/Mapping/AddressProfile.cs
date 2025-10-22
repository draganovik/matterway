using AutoMapper;
using Ordering.Api.Features.Addresses.Contracts;
using Ordering.Api.Features.Addresses.Domain;

namespace Ordering.Api.Features.Addresses.Mapping;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressBaseResponse>();
        CreateMap<Address, AddressOrderResponse>();

        CreateMap<AddressBaseRequest, Address>();
    }
}