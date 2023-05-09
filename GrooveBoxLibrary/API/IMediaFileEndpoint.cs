namespace GrooveBoxLibrary.API;

public interface IMediaFileEndpoint
{
    Task CreateMediaFileAsync(MediaFileModel media);
    Task<List<MediaFileModel>> GetAllAsync();
    Task<MediaFileModel> GetMediaFileAsync(string id);
    Task<List<MediaFileModel>> GetUserMediaFilesAsync(string userId);
    Task UpdateMediaFileAsync(MediaFileModel media);
    Task UpdateVoteMediaFileAsync(string mediaFileId, string userId);
}