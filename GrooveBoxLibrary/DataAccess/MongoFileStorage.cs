using MongoDB.Driver.GridFS;

namespace GrooveBoxLibrary.DataAccess;
public class MongoFileStorage : IFileStorage
{
    private readonly GridFSBucket _gridFSBucket;
    private readonly IDbConnection _db;

    public MongoFileStorage(IDbConnection db)
    {
        _db = db;
        _gridFSBucket = new GridFSBucket(_db.Client.GetDatabase(_db.DbName));
    }

    public async Task<ObjectId> StoreFileAsync(Stream fileStream, string fileName)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
                {
                    { "FileName", fileName },
                    { "UploadDate", DateTime.Now }
                }
        };

        return await _gridFSBucket.UploadFromStreamAsync(fileName, fileStream, options);
    }

    public async Task<string> CreateSourcePath(string fileId)
    {
        var fileStream = await GetFileAsync(fileId);
        fileStream.Position = 0; // Reset the stream position to the beginning
        string base64Image = await ConvertStreamToBase64(fileStream);
        return $"data:image/png;base64,{base64Image}";
    }

    private async Task<Stream> GetFileAsync(string fileId)
    {
        var fileStream = new MemoryStream();

        await _gridFSBucket.DownloadToStreamAsync(new ObjectId(fileId), fileStream);
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
