using AutoMapper;
using Customers.API.Features.Customers.Contracts;
using Customers.API.Features.Customers.Domain;

namespace Customers.API.Features.Customers.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerBaseResponse>();
        CreateMap<CustomerBaseRequest, Customer>();
    }
}