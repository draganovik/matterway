using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CustomerModels;

namespace Customers.API.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerBaseResponseModel>();

        CreateMap<CustomerCreateRequestModel, Customer>();
        CreateMap<CustomerUpdateRequestModel, Customer>();
    }
}
