namespace GrooveBoxLibrary.DataAccess;

public interface IFileStorage
{
    string CreateSourcePath(string path);
    Stream GetFile(string fileId);
    Task<ObjectId> StoreFileAsync(Stream fileStream, string fileName);
}