namespace GrooveBoxLibrary.DataAccess;

public interface IFileStorage
{
    Task<string> CreateSourcePath(string path);
    Task<string> StoreFileAsync(Stream fileStream, string fileName);
}