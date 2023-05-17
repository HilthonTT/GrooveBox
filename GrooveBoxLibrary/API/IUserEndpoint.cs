namespace GrooveBoxLibrary.API;

public interface IUserEndpoint
{
    Task CreateUserAsync(CreateUserModel user);
    Task<UserModel> GetByIdAsync(string userId);
    Task UpdateUserAsync(UserModel user);
    Task UpdateUserSubscriptionAsync(string authorId, string userId);
}