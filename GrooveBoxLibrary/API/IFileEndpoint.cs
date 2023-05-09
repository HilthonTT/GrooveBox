namespace GrooveBoxLibrary.API;

public interface IFileEndpoint
{
    Task<string> GetSourcePathAsync(string fileId);
    Task<ObjectId> StoreFileAsync(FileUploadModel model);
}