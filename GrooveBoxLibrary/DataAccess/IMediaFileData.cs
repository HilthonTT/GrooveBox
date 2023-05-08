namespace GrooveBoxLibrary.DataAccess;

public interface IMediaFileData
{
    Task CreateMediaFileAsync(MediaFileModel mediaFile);
    Task<List<MediaFileModel>> GetAllMediaFilesAsync();
    Task<MediaFileModel> GetMediaFileAsync(string id);
    Task<List<MediaFileModel>> GetUsersMediaFilesAsync(string userId);
    Task UpdateMediaFileAsync(MediaFileModel mediaFile);
    Task UpdateVoteMediaFileAsync(string mediaFileId, string userId);
}