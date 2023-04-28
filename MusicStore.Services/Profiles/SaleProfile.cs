using AutoMapper;
using MusicStore.Dto.Response;
using MusicStore.Entities;

namespace MusicStore.Services.Profiles;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<Sale, SaleDtoResponse>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DateEvent, opt => opt.MapFrom(src => src.Concert.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.TimeEvent, opt => opt.MapFrom(src => src.Concert.DateEvent.ToString("hh:mm tt")))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Concert.Genre.Name))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Concert.ImageUrl))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Concert.Title))
            .ForMember(dest => dest.OperationNumber, opt => opt.MapFrom(src => src.OperationNumber))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.SaleDate.ToString("yyyy-MM-dd hh:mm tt")))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
    }
}