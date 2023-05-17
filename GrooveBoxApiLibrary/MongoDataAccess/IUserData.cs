namespace GrooveBoxApiLibrary.MongoDataAccess;
public interface IUserData
{
    Task CreateUserAsync(MongoUserModel user);
    Task<MongoUserModel> GetUserAsync(string id);
    Task<MongoUserModel> GetUserFromAuthenticationAsync(string objectId);
    Task<List<MongoUserModel>> GetUsersAsync();
    Task UpdateSubscriptionAsync(string authorId, string userId);
    Task UpdateUserAsync(MongoUserModel user);
}