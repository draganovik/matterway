using AutoMapper;
using Catalog.API.Features.ProductDetails.Contracts;
using Catalog.API.Features.ProductDetails.Domain;

namespace Catalog.API.Features.ProductDetails.Mapping;

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