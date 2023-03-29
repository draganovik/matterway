using AutoMapper;
using Identity.API.Entities;
using Identity.API.Models;

namespace Identity.API.Profiles;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionPostResponse>()
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.Expires, opt => opt.MapFrom(src => src.Expires));
    }
}
