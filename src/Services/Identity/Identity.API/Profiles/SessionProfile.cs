using AutoMapper;
using Identity.API.Entities;
using Identity.API.Models.SessionModels;

namespace Identity.API.Profiles;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionBaseResponseModel>();
    }
}
