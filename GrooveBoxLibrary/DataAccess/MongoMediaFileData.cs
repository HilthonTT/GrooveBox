namespace GrooveBoxLibrary.DataAccess;
public class MongoMediaFileData : IMediaFileData
{
    private readonly IDbConnection _db;
    private readonly IUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<MediaFileModel> _mediaFiles;
    private const string CacheName = "MediaFileData";

    public MongoMediaFileData(IDbConnection db,
                              IUserData userData,
                              IMemoryCache cache)
    {
        _db = db;
        _userData = userData;
        _cache = cache;
        _mediaFiles = db.MediaFileCollection;
    }

    public async Task<List<MediaFileModel>> GetUsersMediaFilesAsync(string userId)
    {
        var output = _cache.Get<List<MediaFileModel>>(userId);
        if (output is null)
        {
            var results = await _mediaFiles.FindAsync(m => m.Author.Id == userId);
            output = await results.ToListAsync();

            _cache.Set(userId, output, TimeSpan.FromMinutes(5));
        }

        return output;
    }

    public async Task<List<MediaFileModel>> GetAllMediaFilesAsync()
    {
        var output = _cache.Get<List<MediaFileModel>>(CacheName);
        if (output is null)
        {
            var results = await _mediaFiles.FindAsync(m => m.Archived == false);
            output = await results.ToListAsync();

            _cache.Set(CacheName, output, TimeSpan.FromHours(1));
        }

        return output;
    }

    public async Task<MediaFileModel> GetMediaFileAsync(string id)
    {
        var output = _cache.Get<MediaFileModel>(id);
        if (output is null)
        {
            var results = await _mediaFiles.FindAsync(m => m.Id == id);
            output = await results.FirstOrDefaultAsync();

            _cache.Set(id, output, TimeSpan.FromMinutes(15));
        }

        return output;
    }

    public async Task UpdateMediaFileAsync(MediaFileModel mediaFile)
    {
        await _mediaFiles.ReplaceOneAsync(m => m.Id == mediaFile.Id, mediaFile);
        _cache.Remove(CacheName);
    }

    public async Task UpdateVoteMediaFileAsync(string mediaFileId, string userId)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var mediaFileInTransaction = db.GetCollection<MediaFileModel>(_db.MediaFilesCollectionName);
            var mediaFile = await (await mediaFileInTransaction.FindAsync(m => m.Id == mediaFileId)).FirstAsync();

            bool isUpVote = mediaFile.UserVotes.Add(userId);
            if (isUpVote == false)
            {
                mediaFile.UserVotes.Remove(userId);
            }

            await mediaFileInTransaction.ReplaceOneAsync(session, m => m.Id == mediaFileId, mediaFile);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(userId);

            if (isUpVote)
            {
                user.VotedOnFiles.Add(new BasicMediaFileModel(mediaFile));
            }
            else
            {
                var mediaFileToRemove = user.VotedOnFiles.Where(m => m.Id == mediaFileId).FirstOrDefault();
                user.VotedOnFiles.Remove(mediaFileToRemove);
            }
            await usersInTransaction.ReplaceOneAsync(session, u => u.Id == userId, user);

            await session.CommitTransactionAsync();
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task CreateMediaFileAsync(MediaFileModel mediaFile)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var mediaFileInTransaction = db.GetCollection<MediaFileModel>(_db.MediaFilesCollectionName);
            await mediaFileInTransaction.InsertOneAsync(session, mediaFile);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(mediaFile.Author.Id);
            user.AuthoredFiles.Add(new BasicMediaFileModel(mediaFile));
            await usersInTransaction.ReplaceOneAsync(session, u => u.Id == user.Id, user);

            await session.CommitTransactionAsync();
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
