using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class MediaFileEndpoint : IMediaFileEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<MediaFileEndpoint> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheName = "MediaFilesData";

    public MediaFileEndpoint(IAPIHelper apiHelper,
                             ILogger<MediaFileEndpoint> logger,
                             IMemoryCache cache)
    {
        _apiHelper = apiHelper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<MediaFileModel>> GetAllAsync()
  {
        var output = _cache.Get<List<MediaFileModel>>(CacheName);
        if (output is null)
        {
            using var response = await _apiHelper.ApiClient.GetAsync("api/MediaFile");
            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsAsync<List<MediaFileModel>>();
                _cache.Set(CacheName, output, TimeSpan.FromHours(1));
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        return output;
    }

    public async Task<List<MediaFileModel>> GetUserMediaFilesAsync(string userId)
    {
        var output = _cache.Get<List<MediaFileModel>>(CacheName + userId);
        if (output is null)
        {
            using var response = await _apiHelper.ApiClient.GetAsync($"api/MediaFile/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsAsync<List<MediaFileModel>>();
                _cache.Set(CacheName, output, TimeSpan.FromHours(1));
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        return output;
    }

    public async Task<MediaFileModel> GetMediaFileAsync(string id)
    {
        string key = CacheName + id;
        var output = _cache.Get<MediaFileModel>(key);
        if (output is null)
        {
            using var response = await _apiHelper.ApiClient.GetAsync($"api/MediaFile/media/{id}");
            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsAsync<MediaFileModel>();
                _cache.Set(key, output, TimeSpan.FromHours(1));
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        return output;
    }

    public async Task UpdateMediaFileAsync(MediaFileModel media)
    {
        using var response = await _apiHelper.ApiClient.PutAsJsonAsync($"api/MediaFile/media", media);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("PUT REQUEST SUCCESSFULL");
            _cache.Remove(CacheName + media.Id);
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task UpdateVoteMediaFileAsync(string mediaFileId, string userId)
    {
        var content = new StringContent("");
        using var response = await _apiHelper.ApiClient.PutAsync($"api/MediaFile/media/votes/{mediaFileId}/{userId}", content);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("PUT REQUEST SUCCESSFULL");
            _cache.Remove(CacheName + mediaFileId);
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task CreateMediaFileAsync(MediaFileModel media)
    {
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync($"api/MediaFile", media);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("POST REQUEST SUCCESSFULL");
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
