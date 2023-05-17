using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class UserEndpoint : IUserEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<UserEndpoint> _logger;

    public UserEndpoint(IAPIHelper apiHelper, ILogger<UserEndpoint> logger)
    {
        _apiHelper = apiHelper;
        _logger = logger;
    }

    public async Task<UserModel> GetByObjectIdAsync(string objectId)
    {
        var content = new StringContent("");

        using var response = await _apiHelper.ApiClient.PostAsync($"api/User/GetByObjectId/{objectId}", content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<UserModel>();
            return result;
        }
        _logger.LogError("Error: {response}", response.ReasonPhrase);
        return null;
    }

    public async Task CreateUserAsync(CreateUserModel user)
    {
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Register", user);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User successfully been created.");
            return;
        }
        _logger.LogError("Error: {response}", response.ReasonPhrase);
    }

    public async Task UpdateUserSubscriptionAsync(string authorId, string userId)
    {
        var content = new StringContent("");

        using var response = await _apiHelper.ApiClient.PostAsync($"api/User/UpdateUserSubscription/{authorId}/{userId}", content);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Subscription successfully been updated.");
            return;
        }
        _logger.LogError("Error: {response}", response.ReasonPhrase);
    }

    public async Task UpdateUserAsync(UserModel user)
    {
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/UpdateUser", user);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User successfully been updated.");
            return;
        }
        _logger.LogError("Error: {response}", response.StatusCode);
    }
}
