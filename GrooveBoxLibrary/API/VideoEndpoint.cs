using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GrooveBoxLibrary.API;

public class VideoEndpoint : IVideoEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<VideoEndpoint> _logger;
    private readonly IMemoryCache _cache;

    public VideoEndpoint(IAPIHelper apiHelper,
                         ILogger<VideoEndpoint> logger,
                         IMemoryCache cache)
    {
        _apiHelper = apiHelper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<string> GetVideoUrlAsync(string id)
    {
        var output = _cache.Get<string>(id);
        if (output is null)
        {
            using var response = await _apiHelper.ApiClient.GetAsync($"/api/videos/{id}");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                output = json.Value<string>("url");

                _cache.Set(id, output, TimeSpan.FromHours(5));
            }
            else
            {
                _logger.LogError("GET request failed with status code: {response.StatusCode}", response.StatusCode);
                return "";
            }
        }

        return output;
    }
}
