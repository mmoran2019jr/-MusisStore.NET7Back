using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IGenreService
{
    Task<BaseResponseGeneric<IEnumerable<GenreDtoResponse>>> ListAsync();

    Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(int id);

    Task<BaseResponseGeneric<int>> AddAsync(GenreDtoRequest request);

    Task<BaseResponse> UpdateAsync(int id, GenreDtoRequest request);

    Task<BaseResponse> DeleteAsync(int id);
}