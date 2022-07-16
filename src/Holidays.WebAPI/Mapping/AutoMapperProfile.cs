using AutoMapper;
using Holidays.Core.Monads;
using Holidays.Core.OfferModel;
using Holidays.DataTransferObjects;

namespace Holidays.WebAPI.Mapping;

internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Offer, OfferDto>();

        CreateMap<PriceHistory, PriceHistoryDto>()
            .ForMember(
                dest => dest.History,
                opt => opt.MapFrom(src => src.Prices
                    .Select(p => new PriceHistoryEntryDto { Timestamp = p.Key, Price = p.Value })
                    .ToArray()));

        CreateMap(typeof(Maybe<>), typeof(Maybe<>))
            .ConvertUsing(typeof(MaybeConverter<,>));
    }
}
