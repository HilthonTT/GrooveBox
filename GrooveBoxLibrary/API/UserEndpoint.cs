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

    public async Task<UserModel> GetByIdAsync(string userId)
    {
        HttpContent requestContent = new StringContent("");

        using var response = await _apiHelper.ApiClient.PostAsync($"api/User/GetById/{userId}", requestContent);
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
        HttpContent requestContent = new StringContent("");

        using var response = await _apiHelper.ApiClient.PostAsync($"api/User/UpdateUserSubscription/{authorId}/{userId}", requestContent);
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

    public async Task<Dictionary<string, string>> GetAllRolesAsync()
    {
        using var response = await _apiHelper.ApiClient.GetAsync("api/User/Admin/GetAllRoles");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<Dictionary<string, string>>();
            return result;
        }
        throw new Exception(response.ReasonPhrase);
    }

    public async Task CreateRoleAsync(string roleName)
    {
        var data = new { roleName };
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/CreateRole", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The role of name {roleName} has been added to the database", roleName);
            return;
        }
        throw new Exception(response.ReasonPhrase);
    }

    public async Task AddUserToRoleAsync(string userId, string roleName)
    {
        var data = new { userId, roleName };

        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/AddUserToRole", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The user of Id {userId} has gained the role of name {roleName}",
                data.userId, data.roleName);
            return;
        }
        throw new Exception(response.ReasonPhrase);
    }

    public async Task RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var data = new { userId, roleName };

        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/RemoveUserFromRole", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The user of Id {userId} has lost the role of name {roleName}",
                data.userId, data.roleName);
            return;
        }
        throw new Exception(response.ReasonPhrase);
    }
}
