﻿using AutoMapper;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;

namespace MusicStore.Services.Profiles;

public class ConcertProfile : Profile
{
    public ConcertProfile()
    {
        CreateMap<Concert, ConcertDtoResponse>()
            .ForMember(destino => destino.Id, origen => origen.MapFrom(x => x.Id))
            .ForMember(destino => destino.Title, origen => origen.MapFrom(x => x.Title))
            .ForMember(destino => destino.Description, origen => origen.MapFrom(x => x.Description))
            .ForMember(destino => destino.TicketsQuantity, origen => origen.MapFrom(x => x.TicketsQuantity))
            .ForMember(destino => destino.UnitPrice, origen => origen.MapFrom(x => x.UnitPrice))
            .ForMember(destino => destino.Place, origen => origen.MapFrom(x => x.Place))
            .ForMember(destino => destino.ImageUrl, origen => origen.MapFrom(x => x.ImageUrl))
            .ForMember(destino => destino.DateEvent, origen => origen.MapFrom(x => x.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(destino => destino.TimeEvent, origen => origen.MapFrom(x => x.DateEvent.ToString("HH:mm:ss")))
            .ForMember(destino => destino.Genre, origen => origen.MapFrom(x => x.Genre.Name))
            .ForMember(destino => destino.Status, origen => origen.MapFrom(x => x.Status ? "Activo" : "Inactivo"));
        
        
        
        CreateMap<Concert, ConcertSingleDtoResponse>()
            .ForMember(destino => destino.Id, origen => origen.MapFrom(x => x.Id))
            .ForMember(destino => destino.Title, origen => origen.MapFrom(x => x.Title))
            .ForMember(destino => destino.Description, origen => origen.MapFrom(x => x.Description))
            .ForMember(destino => destino.TicketsQuantity, origen => origen.MapFrom(x => x.TicketsQuantity))
            .ForMember(destino => destino.UnitPrice, origen => origen.MapFrom(x => x.UnitPrice))
            .ForMember(destino => destino.Place, origen => origen.MapFrom(x => x.Place))
            .ForMember(destino => destino.ImageUrl, origen => origen.MapFrom(x => x.ImageUrl))
            .ForMember(destino => destino.DateEvent, origen => origen.MapFrom(x => x.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(destino => destino.TimeEvent, origen => origen.MapFrom(x => x.DateEvent.ToString("HH:mm:ss")))
            .ForMember(destino => destino.GenreDtoResponse, origen => origen.MapFrom(x => new GenreDtoResponse
            {
                Id = x.GenreId,
                Name = x.Genre.Name,
                Status = x.Genre.Status
            }))
            .ForMember(destino => destino.Status, origen => origen.MapFrom(x => x.Status ? "Activo" : "Inactivo"));

        CreateMap<ConcertDtoRequest, Concert>()
            .ForMember(destino => destino.GenreId, origen => origen.MapFrom(x => x.IdGenre))
            .ForMember(destino => destino.Title, origen => origen.MapFrom(x => x.Title))
            .ForMember(destino => destino.Description, origen => origen.MapFrom(x => x.Description))
            .ForMember(destino => destino.TicketsQuantity, origen => origen.MapFrom(x => x.TicketsQuantity))
            .ForMember(destino => destino.UnitPrice, origen => origen.MapFrom(x => x.UnitPrice))
            .ForMember(destino => destino.Place, origen => origen.MapFrom(x => x.Place))
            .ForMember(destino => destino.DateEvent,
                origen => origen.MapFrom(x => DateTime.Parse($"{x.DateEvent} {x.TimeEvent}")));
    }
}