using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _repository;
    private readonly ILogger<GenreService> _logger;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepository repository, ILogger<GenreService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<IEnumerable<GenreDtoResponse>>> ListAsync()
    {
        var response = new BaseResponseGeneric<IEnumerable<GenreDtoResponse>>();

        try
        {
            response.Data = _mapper.Map<IEnumerable<GenreDtoResponse>>(await _repository.ListAsync());
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in GenreService.ListAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al listar Generos";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(int id)
    {
        var response = new BaseResponseGeneric<GenreDtoResponse>();

        try
        {
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                response.Success = false;
                response.ErrorMessage = "No se encontro el Genero";
                return response;
            }

            response.Data = _mapper.Map<GenreDtoResponse>(entity);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in GenreService.FindByIdAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al buscar el Genero";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> AddAsync(GenreDtoRequest request)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var id = await _repository.AddAsync(_mapper.Map<Genre>(request));

            response.Data = id;
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in GenreService.AddAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al agregar el Genero";
        }

        return response;
    }

    public async Task<BaseResponse> UpdateAsync(int id, GenreDtoRequest request)
    {
        var response = new BaseResponse();

        try
        {
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                response.Success = false;
                response.ErrorMessage = "No se encontro el Genero";
                return response;
            }

            _mapper.Map(request, entity);

            await _repository.UpdateAsync();

            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in GenreService.UpdateAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al actualizar el Genero";
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(int id)
    {
        var response = new BaseResponse();

        try
        {
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                response.Success = false;
                response.ErrorMessage = "No se encontro el Genero";
                return response;
            }

            await _repository.DeleteAsync(entity.Id);

            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in GenreService.DeleteAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al eliminar el Genero";
        }

        return response;
    }
}