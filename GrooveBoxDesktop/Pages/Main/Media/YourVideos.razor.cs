using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Media;

public partial class YourVideos
{
    private List<MediaFileModel> medias;
    private string searchText = "";
    protected override async Task OnInitializedAsync()
    {
        medias = await mediaFileEndpoint.GetUserMediaFilesAsync(loggedInUser.Id);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FilterMedias();
            StateHasChanged();
        }
    }

    private async Task FilterMedias()
    {
        var output = await mediaFileEndpoint.GetUserMediaFilesAsync(loggedInUser.Id);
        if (string.IsNullOrWhiteSpace(searchText)is false)
        {
            output = output.Where(m => m.Title.Contains(
                searchText, StringComparison.InvariantCultureIgnoreCase) || 
                m.Description.Contains(
                    searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        medias = output;
    }

    private async Task OnSearchInput(string searchInput)
    {
        searchText = searchInput;
        await FilterMedias();
    }

    private void LoadResults()
    {
        if (string.IsNullOrWhiteSpace(searchText)is false)
        {
            navManager.NavigateTo($"/results/{searchText}");
        }
        else
        {
            navManager.NavigateTo($"/");
        }
    }
}