namespace GrooveBoxLibrary.DataAccess;
public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;
    private readonly IDbConnection _db;

    public MongoUserData(IDbConnection db)
    {
        _users = db.UserCollection;
        _db = db;
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var results = await _users.FindAsync(_ => true);
        return await results.ToListAsync();
    }

    public async Task<UserModel> GetUserAsync(string id)
    {
        var results = await _users.FindAsync(u => u.Id == id);
        return await results.FirstOrDefaultAsync();
    }

    public async Task<UserModel> GetUserFromAuthenticationAsync(string objectId)
    {
        var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
        return await results.FirstOrDefaultAsync();
    }

    public Task CreateUserAsync(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUserAsync(UserModel user)
    {
        var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }

    public async Task UpdateSubscriptionAsync(string authorId, string userId)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var authorsInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var author = await (await authorsInTransaction.FindAsync(a => a.Id == authorId)).FirstAsync();

            bool isSubscribed = author.UserSubscriptions.Add(userId);
            if (isSubscribed is false)
            {
                author.UserSubscriptions.Remove(userId);
            }

            await authorsInTransaction.ReplaceOneAsync(session, a => a.Id == authorId, author);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await GetUserAsync(userId);

            if (isSubscribed)
            {
                user.SubscribedAuthors.Add(new BasicUserModel(author));
            }
            else
            {
                var authorToRemove = user.SubscribedAuthors.Where(m => m.Id == authorId).FirstOrDefault();
                user.SubscribedAuthors.Remove(authorToRemove);
            }
            await usersInTransaction.ReplaceOneAsync(session, u => u.Id == userId, user);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
