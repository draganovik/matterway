using AutoMapper;
using Identity.API.Features.Sessions.Contracts;
using Identity.API.Features.Sessions.Domain;

namespace Identity.API.Features.Sessions.Mapping;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionBaseResponse>();
    }
}