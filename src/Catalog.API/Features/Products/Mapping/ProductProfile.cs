using AutoMapper;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Domain;

namespace Catalog.Api.Features.Products.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductBaseResponse>();
        CreateMap<ProductBaseRequest, Product>();
    }
}