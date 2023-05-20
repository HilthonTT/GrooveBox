using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace GrooveBoxApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VideosController : ControllerBase
{
    private readonly GridFSBucket _gridFSBucket;
    public VideosController(IMongoDatabase database)
    {
        _gridFSBucket = new GridFSBucket(database);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVideo(string id)
    {
        var objectId = new ObjectId(id);

        // Retrieve the video file from MongoDB based on the provided id
        var files = await _gridFSBucket.FindAsync(new BsonDocument("_id", objectId));
        var file = await files.FirstOrDefaultAsync();

        if (file is not null)
        {
            // Generate a URL for accessing the video file
            var url = Url.Action("StreamVideo", "Videos", new { id = objectId.ToString() }, Request.Scheme);

            return Ok(new { url });
        }

        return NotFound(); // Or handle the case when the video file is not found
    }

    [HttpGet("stream/{id}")]
    public async Task<IActionResult> StreamVideo(string id)
    {
        var objectId = new ObjectId(id);

        // Retrieve the video file from MongoDB based on the provided id
        var stream = await _gridFSBucket.OpenDownloadStreamAsync(objectId);

        // Determine the content type based on the file extension or set it explicitly for specific formats
        string contentType;
        if (stream.FileInfo.Filename.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
        {
            contentType = "video/mp4";
        }
        else if (stream.FileInfo.Filename.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
        {
            contentType = "video/x-matroska";
        }
        else
        {
            contentType = "video/*";
        }

        // Return the video file as a FileStreamResult with the determined content type
        return new FileStreamResult(stream, contentType);
    }
}
