using AutoMapper;
using Identity.API.Entities;
using Identity.API.Models.SystemUserModels;

namespace Identity.API.Profiles;

public class SystemUserProfile : Profile
{
    public SystemUserProfile()
    {
        CreateMap<SystemUser, SystemUserBaseResponseModel>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
           .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<SystemUserBaseRequestModel, SystemUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
    }
}
