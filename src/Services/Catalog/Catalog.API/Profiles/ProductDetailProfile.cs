using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;

namespace Catalog.API.Profiles;

public class ProductDetailProfile : Profile
{
    public ProductDetailProfile()
    {
        CreateMap<ProductDetail, ProductDetailProductResponseModel>();
        CreateMap<ProductDetail, ProductDetailBaseResponseModel>()
            .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Product != null ? src.Product.Title : null));

        CreateMap<ProductDetailBaseRequestModel, ProductDetail>();
    }
}
