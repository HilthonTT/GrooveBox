using GrooveBoxApiLibrary.Models;
using GrooveBoxApiLibrary.MongoDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveBoxApi.Controllers;
[Route("api/[controller]")]
[ApiController]

public class MediaFileController : ControllerBase
{
    private readonly IMediaFileData _mediaFileData;

    public MediaFileController(IMediaFileData mediaFileData)
    {
        _mediaFileData = mediaFileData;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<List<MediaFileModel>> GetMediaFilesAsync()
    {
        return await _mediaFileData.GetAllMediaFilesAsync();
    }

    [HttpGet("user/{userId}")]
    public async Task<List<MediaFileModel>> GetUserMediaFilesAsync(string userId)
    {
        return await _mediaFileData.GetUsersMediaFilesAsync(userId);
    }

    [HttpGet("media/{id}")]
    [AllowAnonymous]
    public async Task<MediaFileModel> GetMediaFileAsync(string id)
    {
        return await _mediaFileData.GetMediaFileAsync(id);
    }

    [HttpPut("media")]
    public async Task UpdateMediaFileAsync(MediaFileModel media)
    {
        await _mediaFileData.UpdateMediaFileAsync(media);
    }

    [HttpPut("media/votes/{mediaFileId}/{userId}")]
    public async Task UpdateVoteMediaFileAsync(string mediaFileId, string userId)
    {
        await _mediaFileData.UpdateVoteMediaFileAsync(mediaFileId, userId);
    }

    [HttpPost]
    public async Task CreateMediaFileAsync(MediaFileModel media)
    {
        await _mediaFileData.CreateMediaFileAsync(media);
    }
}
