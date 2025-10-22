using AutoMapper;
using Ordering.API.Features.Addresses.Contracts;
using Ordering.API.Features.Addresses.Domain;

namespace Ordering.API.Features.Addresses.Mapping;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressBaseResponse>();
        CreateMap<Address, AddressOrderResponse>();

        CreateMap<AddressBaseRequest, Address>();
    }
}