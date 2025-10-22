using AutoMapper;
using Identity.Api.Features.Sessions.Contracts;
using Identity.Api.Features.Sessions.Domain;

namespace Identity.Api.Features.Sessions.Mapping;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionBaseResponse>();
    }
}