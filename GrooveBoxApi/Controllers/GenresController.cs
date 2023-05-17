using GrooveBoxApiLibrary.Models;
using GrooveBoxApiLibrary.MongoDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveBoxApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly IGenreData _genreData;

    public GenresController(IGenreData genreData)
    {
        _genreData = genreData;
    }

    [HttpGet]
    public async Task<List<GenreModel>> GetGenresAsync()
    {
        return await _genreData.GetAllGenresAsync();
    }

    [HttpPost]
    [Authorize]
    public async Task CreateGenreAsync(GenreModel genre)
    {
        await _genreData.CreateGenreAsync(genre);
    }
}
