using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admins")]
public class ReportsController : ControllerBase
{
    private readonly ISaleService _service;

    public ReportsController(ISaleService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponseGeneric<ICollection<ReportDtoResponse>>), 200)]
    public async Task<IActionResult> GetReportSales(string dateStart, string dateEnd)
    {
        try
        {
            var response = await _service.GetReportSaleAsync(DateTime.Parse(dateStart), DateTime.Parse(dateEnd));

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}