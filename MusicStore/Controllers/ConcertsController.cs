using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Policy = "Admins")]
[Authorize]
public class ConcertsController : ControllerBase
{
    private readonly IConcertService _service;
    private readonly ILogger<ConcertsController> _logger;

    public ConcertsController(IConcertService service, ILogger<ConcertsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET api/Concerts?page=1&rows=10

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponsePagination<ConcertDtoResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponsePagination<ConcertDtoResponse>), 404)]
    public async Task<IActionResult> ListAsync(string? filter, int page = 1, int rows = 10)
    {
        var response = await _service.ListAsync(filter, page, rows);

        if (filter == null)
            _logger.LogWarning("Se realizo una busqueda sin filtros");

        return response.Success ? Ok(response) : NotFound(response);
    }

    // POST api/Concerts

    [HttpPost]
    [Authorize(Policy = "Admins")]
    [ProducesResponseType(typeof(BaseResponseGeneric<int>), 200)]
    [ProducesResponseType(typeof(BaseResponseGeneric<int>), 400)]
    public async Task<IActionResult> AddAsync([FromBody] ConcertDtoRequest request)
    {
        var response = await _service.AddAsync(request);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    // GET api/Concerts/5

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BaseResponseGeneric<ConcertSingleDtoResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponseGeneric<ConcertSingleDtoResponse>), 404)]
    public async Task<IActionResult> FindByIdAsync(int id)
    {
        var response = await _service.FindByIdAsync(id);

        return response.Success ? Ok(response) : NotFound(response);
    }

    // PUT api/Concerts/5
    [HttpPut("{id:int}")]
    [Authorize(Policy = "Admins")]
    [ProducesResponseType(typeof(BaseResponse), 200)]
    [ProducesResponseType(typeof(BaseResponse), 400)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ConcertDtoRequest request)
    {
        var response = await _service.UpdateAsync(id, request);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    // DELETE api/Concerts/5
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Admins")]
    [ProducesResponseType(typeof(BaseResponse), 200)]
    [ProducesResponseType(typeof(BaseResponse), 404)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _service.DeleteAsync(id);

        return response.Success ? Ok(response) : NotFound(response);
    }

    // PATCH api/Concerts/5

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "Admins")]
    [ProducesResponseType(typeof(BaseResponse), 200)]
    [ProducesResponseType(typeof(BaseResponse), 404)]
    public async Task<IActionResult> FinalizeAsync(int id)
    {
        var response = await _service.FinalizeAsync(id);

        return response.Success ? Ok(response) : NotFound(response);
    }

}