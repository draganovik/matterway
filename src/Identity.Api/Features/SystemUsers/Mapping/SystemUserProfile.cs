using AutoMapper;
using Identity.Api.Features.SystemUsers.Contracts;
using Identity.Api.Features.SystemUsers.Domain;

namespace Identity.Api.Features.SystemUsers.Mapping;

public class SystemUserProfile : Profile
{
    public SystemUserProfile()
    {
        CreateMap<SystemUser, SystemUserBaseResponse>();
        CreateMap<SystemUserBaseRequest, SystemUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}