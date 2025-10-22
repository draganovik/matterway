using AutoMapper;
using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Domain;

namespace Catalog.Api.Features.ProductDetails.Mapping;

public class ProductDetailProfile : Profile
{
    public ProductDetailProfile()
    {
        CreateMap<ProductDetail, ProductDetailProductResponse>();
        CreateMap<ProductDetail, ProductDetailBaseResponse>()
            .ForMember(dest => dest.ProductTitle,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.Title : null));

        CreateMap<ProductDetailBaseRequest, ProductDetail>();
    }
}