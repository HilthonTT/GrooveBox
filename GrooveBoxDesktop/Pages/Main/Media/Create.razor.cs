using Microsoft.AspNetCore.Components.Forms;
using GrooveBoxLibrary.Models;
using GrooveBoxDesktop.Models;

namespace GrooveBoxDesktop.Pages.Main.Media;

public partial class Create
{
    private CreateMediaFileModel media = new();
    private List<GenreModel> genres;
    private string errorMessage = "";
    private string thumbnailPath = "";
    private bool isUploading = false;
    private long maxFileSizeThumbnail = 1024 * 1024 * 5; // represents 5MB
    private long maxFileSizeMedia = 1024 * 1024 * 1000; // represents 1000MB
    private IBrowserFile thumbnailFile;
    private IBrowserFile mediaFile;
    protected override async Task OnInitializedAsync()
    {
        genres = await genreEndpoint.GetAllAsync();
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
    }

    private async Task OpenUploadAlert()
    {
        await matDialogService.AlertAsync("Remain patient, we are uploading your media! Do not leave the page.");
        return;
    }

    private async Task OpenErrorAlert()
    {
        await matDialogService.AlertAsync(errorMessage);
        return;
    }

    private async Task LoadThumbnailFile(InputFileChangeEventArgs e)
    {
        thumbnailFile = e.File;
        using var stream = thumbnailFile.OpenReadStream(maxFileSizeThumbnail);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        string base64Image = $"data:{thumbnailFile.ContentType};base64,{Convert.ToBase64String(bytes)}";
        thumbnailPath = base64Image;
    }

    private void LoadMediaFile(InputFileChangeEventArgs e)
    {
        mediaFile = e.File;
    }

    private async Task CreateMedia()
    {
        if (await CanUpload()is false)
            return;
        isUploading = true;
        string thumbnailFileId = "";
        string mediaFileId = "";
        await OpenUploadAlert();
        if (thumbnailFile is not null)
        {
            thumbnailFileId = await fileProcessor.CompressAndStoreImageAsync(thumbnailFile);
        }

        using (var mediaStream = mediaFile.OpenReadStream(maxFileSizeMedia))
        {
            mediaFileId = await fileStorage.StoreFileAsync(mediaStream, mediaFile.Name);
            await mediaStream.DisposeAsync();
        }

        MediaFileModel m = new()
        {
            Title = media.Title,
            Description = media.Description,
            ThumbnailPath = thumbnailFileId,
            FilePath = mediaFileId,
            Genre = genres.FirstOrDefault(g => g.Id == media.GenreId),
            Author = new BasicUserModel(loggedInUser),
        };
        await mediaFileEndpoint.CreateMediaFileAsync(m);
        media = new();
        isUploading = false;
        thumbnailFile = null;
        mediaFile = null;
        ClosePage();
    }

    private string GetGenreName()
    {
        if (string.IsNullOrWhiteSpace(media.GenreId)is false)
        {
            var g = genres.FirstOrDefault(g => g.Id == media.GenreId);
            return g.GenreName;
        }

        return "";
    }

    private async Task<bool> CanUpload()
    {
        errorMessage = "";
        if (mediaFile is null)
        {
            errorMessage = "You didn't choose a media to upload.";
            await OpenErrorAlert();
            return false;
        }

        if (mediaFile.Size > maxFileSizeMedia)
        {
            errorMessage = "Your file surpasses the maximum upload size.";
            await OpenErrorAlert();
            return false;
        }

        if (thumbnailFile.Size > maxFileSizeThumbnail)
        {
            errorMessage = "Your thumbnail file surpasses the maximum upload size";
            await OpenErrorAlert();
            return false;
        }

        if (genres.FirstOrDefault(g => g.Id == media.GenreId)is null)
        {
            errorMessage = "Your selected genre is currently unavailable.";
            media.GenreId = "";
            await OpenErrorAlert();
            return false;
        }

        return true;
    }
}