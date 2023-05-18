using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class UserEndpoint : IUserEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<UserEndpoint> _logger;
    private readonly IMemoryCache _cache;

    public UserEndpoint(IAPIHelper apiHelper,
                        ILogger<UserEndpoint> logger,
                        IMemoryCache cache)
    {
        _apiHelper = apiHelper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<UserModel> GetByObjectIdAsync(string objectId)
    {
        var output = _cache.Get<UserModel>(objectId);
        if (output is null)
        {
            var content = new StringContent("");
            using var response = await _apiHelper.ApiClient.PostAsync($"api/User/GetByObjectId/{objectId}", content);
            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsAsync<UserModel>();
                
                _cache.Set(objectId, output, TimeSpan.FromHours(1));
            }
            else
            {
                _logger.LogError("Error: {response}", response.ReasonPhrase);
                return new();
            }
        }

        return output;
    }

    public async Task CreateUserAsync(CreateUserModel user)
    {
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Register", user);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User successfully been created.");
        }
        else
        {
            _logger.LogError("Error: {response}", response.ReasonPhrase);
        }
    }

    public async Task UpdateUserSubscriptionAsync(string authorId, string userId)
    {
        var content = new StringContent("");
        using var response = await _apiHelper.ApiClient.PostAsync($"api/User/UpdateUserSubscription/{authorId}/{userId}", content);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Subscription successfully been updated.");
        }
        else
        {
            _logger.LogError("Error: {response}", response.ReasonPhrase);
        }
    }

    public async Task UpdateUserAsync(UserModel user)
    {
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/UpdateUser", user);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User successfully been updated.");;
        }
        else
        {
            _logger.LogError("Error: {response}", response.StatusCode);
        }
    }

    public async Task ConfirmEmailAsync(string userId, string token)
    {
        var content = new StringContent("");
        using var response = await _apiHelper.ApiClient.PostAsync($"api/User/ConfirmEmail/{userId}/{token}", content);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User of ID {userId} has confirmed their email.", userId);
        }
        else
        {
            _logger.LogError("Error: {response}", response.StatusCode);
        }
    }

    public async Task ForgotPasswordAsync(string token, string email, string password)
    {
        var data = new { Token = token, EmailAddress = email, Password = password };
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/ForgotPassword", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User of email {email} has tempted to reset their password", email);
        }
        else
        {
            _logger.LogError("Error: {response}", response.StatusCode);
        }
    }

    public async Task ResetEmailAsync(string objectId, string newEmail)
    {
        var data = new { UserId = objectId, NewEmail = newEmail };
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/ResetEmail", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User of ObjectId {objectId} has tempted to reset their password", objectId);
        }
        else
        {
            _logger.LogError("Error: {response}", response.StatusCode);
        }
    }
}
