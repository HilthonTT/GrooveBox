﻿using Microsoft.Extensions.Logging;

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

    public async Task<UserModel> GetByIdAsync(string id)
    {
        var data = new { id };

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/GetById", data);
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
        var data = new
        {
            user.ObjectIdentifier,
            user.FirstName,
            user.LastName,
            user.DisplayName,
            user.EmailAddress,
            user.Password
        };

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Register", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User successfully been created.");
            return;
        }
        _logger.LogError("Error: {response}", response.ReasonPhrase);
    }

    public async Task UpdateUserAsync(UserModel user)
    {
        var data = new
        {
            user.FirstName,
            user.LastName,
            user.EmailAddress,
            user.DisplayName
        };

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/UpdateUser", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User successfully been updated.");
            return;
        }
        _logger.LogError("Error: {response}", response.StatusCode);
    }

    public async Task<Dictionary<string, string>> GetAllRolesAsync()
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/User/Admin/GetAllRoles");
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
        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/CreateRole", data);
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

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/AddUserToRole", data);
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

        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/User/Admin/RemoveUserFromRole", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The user of Id {userId} has lost the role of name {roleName}",
                data.userId, data.roleName);
            return;
        }
        throw new Exception(response.ReasonPhrase);
    }
}
