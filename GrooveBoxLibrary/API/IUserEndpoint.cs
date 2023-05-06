namespace GrooveBoxLibrary.API;

public interface IUserEndpoint
{
    Task AddUserToRoleAsync(string userId, string roleName);
    Task CreateRoleAsync(string roleName);
    Task CreateUserAsync(CreateUserModel user);
    Task<Dictionary<string, string>> GetAllRolesAsync();
    Task<UserModel> GetByIdAsync(string id);
    Task RemoveUserFromRoleAsync(string userId, string roleName);
    Task UpdateUserAsync(UserModel user);
}