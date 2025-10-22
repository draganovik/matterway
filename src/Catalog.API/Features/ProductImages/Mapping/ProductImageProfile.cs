using AutoMapper;
using Catalog.Api.Features.ProductImages.Contracts;
using Catalog.Api.Features.ProductImages.Domain;

namespace Catalog.Api.Features.ProductImages.Mapping;

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