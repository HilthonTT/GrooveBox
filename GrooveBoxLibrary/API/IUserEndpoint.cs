namespace GrooveBoxLibrary.API;

public interface IUserEndpoint
{
    Task ConfirmEmailAsync(string userId, string token);
    Task CreateUserAsync(CreateUserModel user);
    Task ForgotPasswordAsync(string email);
    Task<UserModel> GetByObjectIdAsync(string userId);
    Task ResetEmailAsync(string objectId, string newEmail);
    Task UpdateUserAsync(UserModel user);
    Task UpdateUserSubscriptionAsync(string authorId, string userId);
}