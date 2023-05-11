using GrooveBoxLibrary.API;
using MongoDB.Driver.GridFS;

namespace GrooveBoxLibrary.DataAccess;
public class MongoFileStorage : IFileStorage
{
    private readonly IMemoryCache _cache;
    private readonly IConnectionStringEndpoint _connectionStringEndpoint;
    private readonly IAPIDbConnection _connection;

    public MongoFileStorage(IConnectionStringEndpoint connectionStringEndpoint,
                            IAPIDbConnection connection,
                            IMemoryCache cache)
    {
        _connectionStringEndpoint = connectionStringEndpoint;
        _connection = connection;
        _cache = cache;
    }

    public async Task<ObjectId> StoreFileAsync(Stream fileStream, string fileName)
    {
        var conn = await _connection.CreateAsync(_connectionStringEndpoint);
        GridFSBucket gridFSBucket = new(conn.Client.GetDatabase(conn.DbName));

        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    { "FileName", fileName },
                    { "UploadDate", DateTime.Now }
                }
        };

        return await gridFSBucket.UploadFromStreamAsync(fileName, fileStream, options);
    }

    public async Task<string> CreateSourcePath(string fileId)
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            return "";
        }

        string output = _cache.Get<string>(fileId);
        if (output is null)
        {
            var fileStream = await GetFileAsync(fileId);
            fileStream.Position = 0; // Reset the stream position to the beginning
            string base64Image = await ConvertStreamToBase64(fileStream);
            output =  $"data:image/png;base64,{base64Image}";

            _cache.Set(fileId, output, TimeSpan.FromHours(1));
        }

        return output;
    }

    private async Task<Stream> GetFileAsync(string fileId)
    {
        var conn = await _connection.CreateAsync(_connectionStringEndpoint);
        GridFSBucket gridFSBucket = new(conn.Client.GetDatabase(conn.DbName));

        var fileStream = new MemoryStream();
        await gridFSBucket.DownloadToStreamAsync(new ObjectId(fileId), fileStream);
        fileStream.Position = 0;
        return fileStream;
    }

    private static async Task<string> ConvertStreamToBase64(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }
}
