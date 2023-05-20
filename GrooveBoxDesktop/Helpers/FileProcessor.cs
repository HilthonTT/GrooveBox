using GrooveBoxLibrary.DataAccess;
using Microsoft.AspNetCore.Components.Forms;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace GrooveBoxDesktop.Helpers;
public class FileProcessor : IFileProcessor
{
    private const int MaxFileSizeImage = 5 * 1024 * 1024; // 5 MB
    private const int TargetImageWidth = 800;
    private const int TargetImageHeight = 600;

    private readonly IFileStorage _fileStorage;

    public FileProcessor(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<string> CompressAndStoreImageAsync(IBrowserFile imageFile)
    {
        using var imageStream = imageFile.OpenReadStream(MaxFileSizeImage);
        using var image = await Image.LoadAsync(imageStream);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(TargetImageWidth, TargetImageHeight),
            Mode = ResizeMode.Max,
        }));

        using var compressedStream = new MemoryStream();
        image.Save(compressedStream, new PngEncoder());

        compressedStream.Position = 0;

        string fileId = await _fileStorage.StoreFileAsync(compressedStream, imageFile.Name);
        return fileId;
    }
}
