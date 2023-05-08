namespace GrooveBoxLibrary.DataAccess;

public interface IFileStorage
{
    string CreateSourcePathImage(string path);
    string CreateSourcePathVideo(string fileId);
    Task<ObjectId> StoreFileAsync(Stream fileStream, string fileName);
}