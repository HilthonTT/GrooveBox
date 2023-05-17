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

    public async Task<string> StoreFileAsync(Stream fileStream, string fileName)
    {
        var gridFSBucket = await GetBucketAsync();
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    { "FileName", fileName },
                    { "UploadDate", DateTime.Now }
                }
        };

        var objectId = await gridFSBucket.UploadFromStreamAsync(fileName, fileStream, options);
        return objectId.ToString();
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
        var gridFSBucket = await GetBucketAsync();

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

    private async Task<GridFSBucket> GetBucketAsync()
    {
        string key = "GridFSBucket";
        var output = _cache.Get<GridFSBucket>(key);
        if (output is null)
        {
            var conn = await _connection.CreateAsync(_connectionStringEndpoint);
            output = new GridFSBucket(conn.Client.GetDatabase(conn.DbName));
            
            _cache.Set(key, output, TimeSpan.FromHours(5));
        }

        return output;
    }
}
