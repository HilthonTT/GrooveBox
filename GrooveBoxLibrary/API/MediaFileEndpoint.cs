using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class MediaFileEndpoint : IMediaFileEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<MediaFileEndpoint> _logger;

    public MediaFileEndpoint(IAPIHelper apiHelper, ILogger<MediaFileEndpoint> logger)
    {
        _apiHelper = apiHelper;
        _logger = logger;
    }

    public async Task<List<MediaFileModel>> GetAllAsync()
    {
        using var response = await _apiHelper.ApiClient.GetAsync("api/MediaFile");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<List<MediaFileModel>>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task<List<MediaFileModel>> GetUserMediaFilesAsync(string userId)
    {
        using var response = await _apiHelper.ApiClient.GetAsync($"api/MediaFile/user/{userId}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<List<MediaFileModel>>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task<MediaFileModel> GetMediaFileAsync(string id)
    {
        using var response = await _apiHelper.ApiClient.GetAsync($"api/MediaFile/media/{id}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<MediaFileModel>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task UpdateMediaFileAsync(MediaFileModel media)
    {
        using var response = await _apiHelper.ApiClient.PutAsJsonAsync($"api/MediaFile/media", media);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("PUT REQUEST SUCCESSFULL");
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
