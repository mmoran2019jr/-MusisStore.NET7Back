using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class ConcertService : IConcertService
{
    private readonly IConcertRepository _concertRepository;
    private readonly ILogger<ConcertService> _logger;
    private readonly IFileUploader _fileUploader;
    private readonly IMapper _mapper;

    public ConcertService(IConcertRepository concertRepository, 
        ILogger<ConcertService> logger, 
        IFileUploader fileUploader, 
        IMapper mapper)
    {
        _concertRepository = concertRepository;
        _logger = logger;
        _fileUploader = fileUploader;
        _mapper = mapper;
    }

    public async Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows)
    {
        var response = new BaseResponsePagination<ConcertDtoResponse> ();
        try
        {
            var tuple = await _concertRepository
                .ListAsync(x => x.Title.Contains(filter ?? string.Empty),
                    p => _mapper.Map<ConcertDtoResponse>(p),
                    x => x.Title,
                    page, 
                    rows);
            
            response.Data = tuple.Collection;
            response.TotalPages = tuple.Total / rows;
            if (tuple.Total % rows > 0) // Si el residuo es mayor a cero
                response.TotalPages++;

            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar los conciertos {message}", ex.Message);
            response.ErrorMessage = "Error al listar los conciertos";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ConcertSingleDtoResponse>> FindByIdAsync(int id)
    {
        var response = new BaseResponseGeneric<ConcertSingleDtoResponse>();

        try
        {
            var concert = await _concertRepository.FindByIdAsync(id);

            if (concert == null)
            {
                response.ErrorMessage = "No se encontró el concierto";
                return response;
            }

            response.Data = _mapper.Map<ConcertSingleDtoResponse>(concert);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el concierto");
            response.ErrorMessage = "Error al obtener el concierto";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(ConcertDtoRequest request)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var concert = _mapper.Map<Concert>(request);

            concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);

            await _concertRepository.AddAsync(concert);
            response.Data = concert.Id;
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar el concierto {message}", ex.Message);
            response.ErrorMessage = "Error al agregar el concierto";
        }

        return response;
    }

    public async Task<BaseResponse> UpdateAsync(int id, ConcertDtoRequest request)
    {
        var response = new BaseResponse();

        try
        {
            var concert = await _concertRepository.FindByIdAsync(id); // Este SELECT usa el ChangeTracker

            if (concert == null)
            {
                response.ErrorMessage = "No se encontró el concierto";
                return response;
            }

            _mapper.Map(request, concert); // Este UPDATE usa el ChangeTracker

            if (!string.IsNullOrEmpty(request.FileName))
                concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);

            await _concertRepository.UpdateAsync();
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el concierto {message}", ex.Message);
            response.ErrorMessage = "Error al actualizar el concierto";
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            await _concertRepository.DeleteAsync(id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el concierto");
            response.ErrorMessage = "Error al eliminar el concierto";
        }

        return response;
    }

    public async Task<BaseResponse> FinalizeAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            await _concertRepository.FinalizeAsync(id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al finalizar el concierto");
            response.ErrorMessage = "Error al finalizar el concierto";
        }

        return response;
    }
}