using AutoMapper;
using Matterway.Identity.Api.Features.SystemUsers.Contracts;
using Matterway.Identity.Api.Features.SystemUsers.Domain;

namespace Matterway.Identity.Api.Features.SystemUsers.Mapping;

public class SystemUserProfile : Profile
{
    public SystemUserProfile()
    {
        CreateMap<SystemUser, SystemUserBaseResponse>();
        CreateMap<SystemUserBaseRequest, SystemUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}