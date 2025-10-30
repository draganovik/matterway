using AutoMapper;
using Matterway.Payments.Api.Features.Payments.Contracts;
using Matterway.Payments.Api.Features.Payments.Domain;

namespace Matterway.Payments.Api.Features.Payments.Mapping;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentBaseResponse>()
            .ForMember(dest => dest.CardNumber,
                opt => opt.MapFrom(src =>
                    src.CardNumber != null
                        ? src.CardNumber.Replace(src.CardNumber.Substring(0, src.CardNumber.Length - 4),
                            "****-****-****-")
                        : null));

        CreateMap<PaymentBaseRequest, Payment>();
    }
}