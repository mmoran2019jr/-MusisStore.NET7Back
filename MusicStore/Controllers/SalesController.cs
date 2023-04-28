using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _service;
    private readonly ILogger<SalesController> _logger;

    public SalesController(ISaleService service, ILogger<SalesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponseGeneric<int>), 200)]
    [ProducesResponseType(typeof(BaseResponseGeneric<int>), 400)]
    public async Task<IActionResult> CreateSaleAsync([FromBody] SaleDtoRequest request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        
        var response = await _service.CreateSaleAsync(email, request);

        if (response.Success)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet("ListSales")]
    [ProducesResponseType(typeof(BaseResponsePagination<SaleDtoResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponsePagination<SaleDtoResponse>), 404)]
    public async Task<IActionResult> GetListSales(string? filter, int page = 1, int rows = 10)
    {
        var email = HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
        
        var response = await _service.ListAsync(email, filter, page, rows);

        if (response.Success)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpGet("ListSalesByDate")]
    [Authorize(Policy = "Admins")]
    [ProducesResponseType(typeof(BaseResponsePagination<SaleDtoResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponsePagination<SaleDtoResponse>), 404)]
    public async Task<IActionResult> GetListSalesByDate(string dateStart, string dateEnd, int page = 1, int rows = 10)
    {
        try
        {
            var response = await _service.ListAsync(DateTime.Parse(dateStart), DateTime.Parse(dateEnd), page, rows);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Error en conversion de formato de fecha {message}", ex.Message);
            return BadRequest(new BaseResponse { ErrorMessage = "Error en conversion de formato de fecha" });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BaseResponseGeneric<SaleDtoResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponseGeneric<SaleDtoResponse>), 404)]
    public async Task<IActionResult> GetSaleAsync(int id)
    {
        var response = await _service.GetSaleAsync(id);

        return response.Success ? Ok(response) : NotFound(response);
    }

}