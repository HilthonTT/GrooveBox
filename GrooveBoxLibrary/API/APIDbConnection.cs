using GrooveBoxLibrary.API;

namespace GrooveBoxLibrary.DataAccess;
public class APIDbConnection : IAPIDbConnection
{
    private readonly IConnectionStringEndpoint _connectionStringEndpoint;

    public IMongoDatabase Db { get; private set; }
    public string DbName { get; private set; }
    public MongoClient Client { get; private set; }

    public APIDbConnection(IConnectionStringEndpoint connectionStringEndpoint)
    {
        _connectionStringEndpoint = connectionStringEndpoint;
    }

    public async Task<APIDbConnection> CreateAsync(IConnectionStringEndpoint connectionStringEndpoint)
    {
        var connection = await connectionStringEndpoint.GetConnectionStrings();
        var client = new MongoClient(connection.ConnectionString);
        var database = client.GetDatabase(connection.DatabaseName);
        return new APIDbConnection(connectionStringEndpoint)
        {
            Client = client,
            Db = database,
            DbName = connection.DatabaseName
        };
    }
}
