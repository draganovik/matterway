using AutoMapper;
using Ordering.API.Entities;
using Ordering.API.Models.AddressModels;

namespace Ordering.API.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressBaseResponseModel>();
        CreateMap<Address, AddressOrderResponseModel>();

        CreateMap<AddressBaseRequestModel, Address>();
    }
}
