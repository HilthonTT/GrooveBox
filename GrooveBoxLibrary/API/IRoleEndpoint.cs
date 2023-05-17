namespace GrooveBoxLibrary.API;

public interface IRoleEndpoint
{
    Task AddUserToRoleAsync(string userId, string roleName);
    Task CreateRoleAsync(string roleName);
    Task<Dictionary<string, string>> GetAllRolesAsync();
    Task RemoveUserFromRoleAsync(string userId, string roleName);
}