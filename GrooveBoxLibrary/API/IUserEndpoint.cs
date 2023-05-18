namespace GrooveBoxLibrary.API;

public interface IUserEndpoint
{
    Task ConfirmEmailAsync(string userId, string token);
    Task CreateUserAsync(CreateUserModel user);
    Task ForgotPasswordAsync(string token, string email, string password);
    Task<UserModel> GetByObjectIdAsync(string userId);
    Task ResetEmailAsync(string objectId, string newEmail);
    Task UpdateUserAsync(UserModel user);
    Task UpdateUserSubscriptionAsync(string authorId, string userId);
}