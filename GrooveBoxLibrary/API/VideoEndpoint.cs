using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GrooveBoxLibrary.API;

public class VideoEndpoint : IVideoEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<VideoEndpoint> _logger;

    public VideoEndpoint(IAPIHelper apiHelper, ILogger<VideoEndpoint> logger)
    {
        _apiHelper = apiHelper;
        _logger = logger;
    }

    public async Task<string> GetVideoUrlAsync(string id)
    {
        using var response = await _apiHelper.ApiClient.GetAsync($"/api/videos/{id}");
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);
            string url = json.Value<string>("url");
            return url;
        }
        else
        {
            _logger.LogError("GET request failed with status code: {response.StatusCode}", response.StatusCode);
            return null;
        }
    }
}
