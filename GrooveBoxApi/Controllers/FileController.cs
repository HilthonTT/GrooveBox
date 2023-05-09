using GrooveBoxLibrary.DataAccess;
using GrooveBoxLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GrooveBoxApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]

public class FileController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    public FileController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost]
    public async Task<ObjectId> StoreFileAsync([FromForm] FileUploadModel model)
    {
        Stream fileStream = Request.Form.Files[0].OpenReadStream();
        return await _fileStorage.StoreFileAsync(fileStream, model.FileName);
    }

    [HttpGet]
    public async Task<string> CreateSourcePath(string fileId)
    {
        return await _fileStorage.CreateSourcePath(fileId);
    }
}
