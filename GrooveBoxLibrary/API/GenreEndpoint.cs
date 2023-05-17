using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class GenreEndpoint : IGenreEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<GenreEndpoint> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheName = "GenreData";

    public GenreEndpoint(IAPIHelper apiHelper,
                         ILogger<GenreEndpoint> logger,
                         IMemoryCache cache)
    {
        _apiHelper = apiHelper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<GenreModel>> GetAllAsync()
    {
        var output = _cache.Get<List<GenreModel>>(CacheName);
        if (output is null)
        {
            using var response = await _apiHelper.ApiClient.GetAsync("api/Genres");
            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsAsync<List<GenreModel>>();
                _cache.Set(CacheName, output, TimeSpan.FromDays(1));
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        return output;
    }

    public async Task CreateGenreAsync(GenreModel genre)
    {
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/Genres", genre);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Successfully created a genre!");
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
