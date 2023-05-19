using Newtonsoft.Json.Linq;

namespace GrooveBoxLibrary.API;

public class VideoEndpoint : IVideoEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly IMemoryCache _cache;

    public VideoEndpoint(IAPIHelper apiHelper,
                         IMemoryCache cache)
    {
        _apiHelper = apiHelper;
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
                throw new Exception(response.ReasonPhrase);
            }
        }

        return output;
    }
}
