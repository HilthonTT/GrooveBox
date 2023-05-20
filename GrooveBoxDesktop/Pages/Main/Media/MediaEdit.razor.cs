using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using GrooveBoxLibrary.Models;
using GrooveBoxDesktop.Models;

namespace GrooveBoxDesktop.Pages.Main.Media;

public partial class MediaEdit
{
    [Parameter]
    public string Id { get; set; }

    private CreateMediaFileModel editingMedia = new();
    private List<GenreModel> genres;
    private MediaFileModel media;
    private MediaFileModel archivingMedia;
    private IBrowserFile file;
    private long maxFileSize = 1024 * 1024 * 5; // represents 5MB
    private string sourcePath = "";
    private bool isEditDialogOpen = false;
    protected override async Task OnInitializedAsync()
    {
        media = await mediaFileEndpoint.GetMediaFileAsync(Id);
        genres = await genreEndpoint.GetAllAsync();
        if (media is not null)
        {
            if (loggedInUser.Id != media.Author.Id || media.Archived)
            {
                ClosePage();
            }

            editingMedia.Title = media.Title;
            editingMedia.Description = media.Description;
            editingMedia.FilePath = media.FilePath;
            editingMedia.GenreId = media.Genre?.Id;
            sourcePath = await fileStorage.CreateSourcePath(media.ThumbnailPath);
        }
    }

    private async Task LoadFile(InputFileChangeEventArgs e)
    {
        file = e.File;
        using var stream = file.OpenReadStream(maxFileSize);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        string base64Image = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        sourcePath = base64Image;
    }

    private async Task UpdateMedia()
    {
        string filePath = "";
        var genre = genres.FirstOrDefault(g => g.Id == editingMedia.GenreId);
        if (genre is null)
        {
            return;
        }

        if (file is not null)
        {
            using Stream fileStream = file.OpenReadStream(maxFileSize);
            var fileId = await fileStorage.StoreFileAsync(fileStream, file.Name);
            filePath = fileId.ToString();
            await fileStream.DisposeAsync();
        }
        else
        {
            filePath = media.ThumbnailPath;
        }

        media.Title = editingMedia.Title;
        media.Description = editingMedia.Description;
        media.Genre = genre;
        media.ThumbnailPath = filePath;
        await mediaFileEndpoint.UpdateMediaFileAsync(media);
        editingMedia = new();
        ClosePage();
    }

    private async Task ArchiveMedia()
    {
        media.Archived = true;
        await mediaFileEndpoint.UpdateMediaFileAsync(media);
        ClosePage();
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
    }
}