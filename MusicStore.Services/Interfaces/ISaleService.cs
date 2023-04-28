using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface ISaleService
{
    Task<BaseResponseGeneric<int>> CreateSaleAsync(string email, SaleDtoRequest request);

    Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(DateTime dateStart, DateTime dateEnd, int page, int rows);
    
    Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(string email, string? filter, int page, int rows);

    Task<BaseResponseGeneric<ICollection<ReportDtoResponse>>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd);
    
    Task<BaseResponseGeneric<SaleDtoResponse>> GetSaleAsync(int id);
}