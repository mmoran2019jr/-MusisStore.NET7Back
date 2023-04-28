using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IConcertService _concertService;
    private readonly IGenreService _genreService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IConcertService concertService, IGenreService genreService, ILogger<HomeController> logger)
    {
        _concertService = concertService;
        _genreService = genreService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var genres = await _genreService.ListAsync();

            var concerts = await _concertService.ListAsync(null, 1, 100);

            return Ok(new
            {
                Genres = genres.Data,
                Concerts = concerts.Data,
                Success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Error en el Home Controller {message}", ex.Message);

            return Ok(new
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }
}