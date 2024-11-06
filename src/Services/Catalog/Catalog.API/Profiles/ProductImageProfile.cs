using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;

namespace Catalog.API.Profiles;

public class ProductImageProfile : Profile
{
    public ProductImageProfile()
    {
        CreateMap<ProductImage, ProductImageProductResponseModel>();
        CreateMap<ProductImage, ProductImageBaseResponseModel>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.Product != null ? src.Product.Title : null));

        CreateMap<ProductImageBaseRequestModel, ProductImage>();
    }
}