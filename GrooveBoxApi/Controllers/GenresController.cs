using GrooveBoxLibrary.DataAccess;
using GrooveBoxLibrary.Models;
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
}
