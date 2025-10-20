using AutoMapper;
using Catalog.API.Features.ProductImages.Contracts;
using Catalog.API.Features.ProductImages.Domain;

namespace Catalog.API.Features.ProductImages.Mapping;

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