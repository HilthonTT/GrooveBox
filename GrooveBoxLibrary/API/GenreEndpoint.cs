using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class GenreEndpoint : IGenreEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<GenreEndpoint> _logger;

    public GenreEndpoint(IAPIHelper apiHelper, ILogger<GenreEndpoint> logger)
    {
        _apiHelper = apiHelper;
        _logger = logger;
    }

    public async Task<List<GenreModel>> GetAllAsync()
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/Genres");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<List<GenreModel>>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task CreateGenreAsync(GenreModel genre)
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/Genres", genre);
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
