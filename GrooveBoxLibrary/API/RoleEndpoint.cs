using Microsoft.Extensions.Logging;

namespace GrooveBoxLibrary.API;
public class RoleEndpoint : IRoleEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly ILogger<RoleEndpoint> _logger;

    public RoleEndpoint(IAPIHelper apiHelper, ILogger<RoleEndpoint> logger)
    {
        _apiHelper = apiHelper;
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> GetAllRolesAsync()
    {
        using var response = await _apiHelper.ApiClient.GetAsync("api/Role/Admin/GetAllRoles");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<Dictionary<string, string>>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task CreateRoleAsync(string roleName)
    {
        var content = new StringContent("");
        using var response = await _apiHelper.ApiClient.PostAsync($"api/Role/Admin/CreateRole/{roleName}", content);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The role of name {roleName} has been added to the database", roleName);
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task AddUserToRoleAsync(string userId, string roleName)
    {
        var data = new { userId, roleName };
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/Role/Admin/AddUserToRole", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The user of Id {userId} has gained the role of name {roleName}",
                data.userId, data.roleName);
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var data = new { userId, roleName };
        using var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/Role/Admin/RemoveUserFromRole", data);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("The user of Id {userId} has lost the role of name {roleName}",
                data.userId, data.roleName);
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
