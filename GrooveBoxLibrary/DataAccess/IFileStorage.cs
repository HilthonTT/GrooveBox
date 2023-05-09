namespace GrooveBoxLibrary.DataAccess;

public interface IFileStorage
{
    Task<string> CreateSourcePath(string path);
    Task<ObjectId> StoreFileAsync(Stream fileStream, string fileName);
}