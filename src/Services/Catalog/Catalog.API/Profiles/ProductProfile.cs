using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductModels;

namespace Catalog.API.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductBaseResponse>();

        CreateMap<ProductBaseRequest, Product>();
    }
}