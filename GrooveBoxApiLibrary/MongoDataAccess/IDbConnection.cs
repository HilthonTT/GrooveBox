namespace GrooveBoxApiLibrary.MongoDataAccess;
public interface IDbConnection
{
    MongoClient Client { get; }
    string DbName { get; }
    IMongoCollection<GenreModel> GenreCollection { get; }
    string GenreCollectionName { get; }
    IMongoCollection<MediaFileModel> MediaFileCollection { get; }
    string MediaFilesCollectionName { get; }
    IMongoCollection<MongoUserModel> UserCollection { get; }
    string UserCollectionName { get; }
}