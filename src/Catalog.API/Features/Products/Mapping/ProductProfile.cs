using AutoMapper;
using Catalog.API.Features.Products.Contracts;
using Catalog.API.Features.Products.Domain;

namespace Catalog.API.Features.Products.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductBaseResponse>();
        CreateMap<ProductBaseRequest, Product>();
    }
}