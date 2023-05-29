
using trackingApi.Models;
using trackingApi.Dtos;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Currency, CurrencyDto>();
        CreateMap<CurrencyDto, Currency>();

        CreateMap<News, NewsDto>();
        CreateMap<NewsDto, News>();

        CreateMap<Weather, WeatherDto>();
        CreateMap<WeatherDto, Weather>();
    }
}
