using AutoMapper;
using Matterway.Customers.Api.Features.Customers.Contracts;
using Matterway.Customers.Api.Features.Customers.Domain;

namespace Matterway.Customers.Api.Features.Customers.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerBaseResponse>();
        CreateMap<CustomerBaseRequest, Customer>();
    }
}