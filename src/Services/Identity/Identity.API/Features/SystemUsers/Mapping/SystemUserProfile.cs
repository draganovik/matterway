using AutoMapper;
using Identity.API.Features.SystemUsers.Contracts;
using Identity.API.Features.SystemUsers.Domain;

namespace Identity.API.Features.SystemUsers.Mapping;

public class SystemUserProfile : Profile
{
    public SystemUserProfile()
    {
        CreateMap<SystemUser, SystemUserBaseResponse>();
        CreateMap<SystemUserBaseRequest, SystemUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}