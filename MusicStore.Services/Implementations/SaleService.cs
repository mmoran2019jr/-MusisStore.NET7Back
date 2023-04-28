using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repository;
    private readonly IConcertRepository _concertRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<SaleService> _logger;
    private readonly IMapper _mapper;

    public SaleService(ISaleRepository repository, IConcertRepository concertRepository, ICustomerRepository customerRepository, ILogger<SaleService> logger, IMapper mapper)
    {
        _repository = repository;
        _concertRepository = concertRepository;
        _customerRepository = customerRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<BaseResponseGeneric<int>> CreateSaleAsync(string email, SaleDtoRequest request)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            var concert = await _concertRepository.FindByIdAsync(request.ConcertId);

            if (concert == null)
                throw new Exception("No se encontro el concierto");

            if (concert.Finalized)
                throw new Exception("El concierto ya fue finalizado");

            if (concert.DateEvent <= DateTime.Now)
                throw new Exception("El concierto ya terminó");

            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                // Este codigo es de prueba.
                customer = new Customer
                {
                    Email = email,
                    FullName = "Test",
                };
                customer.Id = await _customerRepository.AddAsync(customer);
            }

            var sale = new Sale
            {
                ConcertId = concert.Id,
                CustomerFk = customer.Id,
                Quantity = request.TicketsQuantity,
                Total = concert.UnitPrice * request.TicketsQuantity,
            };

            response.Data = await _repository.CreateSaleAsync(sale);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in SaleService.CreateSaleAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al crear la venta";
        }

        return response;
    }

    public async Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(DateTime dateStart, DateTime dateEnd, int page, int rows)
    {
        var response = new BaseResponsePagination<SaleDtoResponse>();

        try
        {
            var tuple = await _repository.ListAsync(p => p.SaleDate >= dateStart && p.SaleDate <= dateEnd,
                p => _mapper.Map<SaleDtoResponse>(p),
                x => x.OperationNumber
                , page, rows);

            response.Data = tuple.Collection;
            response.TotalPages = tuple.Total / rows;
            if (tuple.Total % rows > 0)
                response.TotalPages++;

            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in SaleService.ListAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al listar las ventas";
        }

        return response;
    }

    public async Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(string email, string? filter, int page, int rows)
    {
        var response = new BaseResponsePagination<SaleDtoResponse>();

        try
        {
            var tuple = await _repository.ListAsync(p => p.Customer.Email.Equals(email)
                                                         && p.Concert.Title.Contains(filter ?? string.Empty),
                p => _mapper.Map<SaleDtoResponse>(p),
                x => x.OperationNumber
                , page, rows);

            response.Data = tuple.Collection;
            response.TotalPages = tuple.Total / rows;
            if (tuple.Total % rows > 0)
                response.TotalPages++;

            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in SaleService.ListAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al listar las ventas";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ReportDtoResponse>>> GetReportSaleAsync(DateTime dateStart, DateTime dateEnd)
    {
        var response = new BaseResponseGeneric<ICollection<ReportDtoResponse>>();

        try
        {
            var list = await _repository.GetReportSaleAsync(dateStart, dateEnd);
            response.Data = list.Select(p => new ReportDtoResponse
            {
                ConcertName = p.ConcertName,
                Total = p.Total
            }).ToList();
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in SaleService.GetReportSaleAsync {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al obtener el reporte de ventas";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<SaleDtoResponse>> GetSaleAsync(int id)
    {
        var response = new BaseResponseGeneric<SaleDtoResponse>();

        try
        {
            var sale = await _repository.GetByIdAsync(id, x => x.Id == id);
            response.Data = _mapper.Map<SaleDtoResponse>(sale);
            response.Success = sale != null;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error al obtener la venta {message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = "Error al obtener la venta";
        }

        return response;
    }
}