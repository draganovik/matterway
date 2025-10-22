using AutoMapper;
using Customers.Api.Features.Customers.Contracts;
using Customers.Api.Features.Customers.Domain;

namespace Customers.Api.Features.Customers.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerBaseResponse>();
        CreateMap<CustomerBaseRequest, Customer>();
    }
}