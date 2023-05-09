using Microsoft.Extensions.Configuration;

namespace GrooveBoxLibrary.DataAccess;
public class DbConnection : IDbConnection
{
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private readonly string _connectionId = "MongoDB";
    public string DbName { get; private set; }
    public string GenreCollectionName { get; private set; } = "genres";
    public string MediaFilesCollectionName { get; private set; } = "media-files";
    public string UserCollectionName { get; private set; } = "users";

    public MongoClient Client { get; private set; }
    public IMongoCollection<GenreModel> GenreCollection { get; private set; }
    public IMongoCollection<MediaFileModel> MediaFileCollection { get; private set; }
    public IMongoCollection<UserModel> UserCollection { get; private set; }

    public DbConnection(IConfiguration config)
    {
        _config = config;
        Client = new MongoClient(_config.GetConnectionString(_connectionId));
        DbName = _config.GetValue<string>("MongoDB:DatabaseName");
        _db = Client.GetDatabase(DbName);

        GenreCollection = _db.GetCollection<GenreModel>(GenreCollectionName);
        MediaFileCollection = _db.GetCollection<MediaFileModel>(MediaFilesCollectionName);
        UserCollection = _db.GetCollection<UserModel>(UserCollectionName);
    }
}
