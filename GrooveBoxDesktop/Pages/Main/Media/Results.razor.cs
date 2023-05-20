using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Media;

public partial class Results
{
    [Parameter]
    public string SearchText { get; set; }

    private List<MediaFileModel> mediaFiles;
    protected override async Task OnInitializedAsync()
    {
        mediaFiles = await mediaFileEndpoint.GetAllAsync();
        mediaFiles = mediaFiles.Where(m => m.Title.Contains(
            SearchText, StringComparison.InvariantCultureIgnoreCase) || 
            m.Description.Contains(
                SearchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
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
        SearchText = await secureStorage.GetAsync(nameof(SearchText)) ?? "";
    }

    private async Task SaveFilterState()
    {
        if (string.IsNullOrWhiteSpace(SearchText)is false)
        {
            await secureStorage.SetAsync(nameof(SearchText), SearchText);
        }
    }

    private async Task OnSearchInput(string searchInput)
    {
        SearchText = searchInput;
        await SaveFilterState();
    }

    private void LoadResults()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            navManager.NavigateTo($"/");
        }
        else
        {
            navManager.NavigateTo($"/results/{SearchText}", true);
        }
    }
}