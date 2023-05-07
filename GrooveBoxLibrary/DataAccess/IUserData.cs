namespace GrooveBoxLibrary.DataAccess;

public interface IUserData
{
    Task CreateUserAsync(UserModel user);
    Task<UserModel> GetUserAsync(string id);
    Task<UserModel> GetUserFromAuthenticationAsync(string objectId);
    Task<List<UserModel>> GetUsersAsync();
    Task UpdateSubscriptionAsync(string authorId, string userId);
}