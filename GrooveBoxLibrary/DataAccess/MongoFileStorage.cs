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

    public Stream GetFile(string fileId)
    {
        var fileStream = new MemoryStream();
        _gridFSBucket.DownloadToStream(new ObjectId(fileId), fileStream);
        fileStream.Position = 0;
        return fileStream;
    }

    public string CreateSourcePath(string fileId)
    {
        var fileStream = GetFile(fileId);
        fileStream.Position = 0; // Reset the stream position to the beginning
        string base64Image = ConvertStreamToBase64(fileStream);
        return $"data:image/png;base64,{base64Image}";
    }

    private static string ConvertStreamToBase64(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }
}
