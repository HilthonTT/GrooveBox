using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Media;

public partial class Genre
{
    [Parameter]
    public string GenreName { get; set; }

    private string searchText = "";
    private List<MediaFileModel> mediaFiles;
    protected override async Task OnInitializedAsync()
    {
        mediaFiles = await mediaFileEndpoint.GetAllAsync();
        mediaFiles = mediaFiles.Where(m => m.Genre.GenreName.ToLower() == GenreName.ToLower()).ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSInterop.InvokeVoidAsync("initializeTooltips");
            await LoadFilterState();
            StateHasChanged();
        }
    }

    private async Task LoadFilterState()
    {
        searchText = await secureStorage.GetAsync(nameof(searchText));
    }

    private async Task SaveFilterState()
    {
        if (string.IsNullOrWhiteSpace(searchText)is false)
        {
            await secureStorage.SetAsync(nameof(searchText), searchText);
        }
    }

    private async Task OnSearchInput(string searchInput)
    {
        searchText = searchInput;
        await SaveFilterState();
    }

    private void LoadResults()
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            navManager.NavigateTo("/");
        }
        else
        {
            navManager.NavigateTo($"/GenreResults/{GenreName}/{searchText}", true);
        }
    }
}