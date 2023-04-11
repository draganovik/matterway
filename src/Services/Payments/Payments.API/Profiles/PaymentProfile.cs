using AutoMapper;
using Payments.API.Entities;
using Payments.API.Models.PaymentModels;

namespace Payments.API.Profiles;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        // map CardNumber string to a format where all but the last 4 digits are replaced with asterisks
        CreateMap<Payment, PaymentBaseResponseModel>()
            .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => src.CardNumber != null ? src.CardNumber.Replace(src.CardNumber.Substring(0, src.CardNumber.Length - 4), "****-****-****-") : null));

        CreateMap<PaymentBaseRequestModel, Payment>();
    }
}
