using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;

namespace Catalog.API.Profiles;

public class ProductImageProfile : Profile
{
    public ProductImageProfile()
    {
        CreateMap<ProductImage, ProductImageProductResponse>();
        CreateMap<ProductImage, ProductImageBaseResponse>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.Title : null));

        CreateMap<ProductImageBaseRequest, ProductImage>();
    }
}