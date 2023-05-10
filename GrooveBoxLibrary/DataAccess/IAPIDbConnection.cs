using GrooveBoxLibrary.API;

namespace GrooveBoxLibrary.DataAccess;
public interface IAPIDbConnection
{
    MongoClient Client { get; }
    string DbName { get; }
    IMongoDatabase Db { get; }

    Task<APIDbConnection> CreateAsync(IConnectionStringEndpoint connectionStringEndpoint);
}