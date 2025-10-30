using AutoMapper;
using Matterway.Identity.Api.Features.Sessions.Contracts;
using Matterway.Identity.Api.Features.Sessions.Domain;

namespace Matterway.Identity.Api.Features.Sessions.Mapping;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionBaseResponse>();
    }
}