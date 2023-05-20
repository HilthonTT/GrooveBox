using Microsoft.JSInterop;
using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Media
{
    public partial class Index
    {
        private List<MediaFileModel> mediaFiles;
        private List<GenreModel> genres;
        private string searchText = "";
        private string selectedGenre = "All";
        protected override async Task OnInitializedAsync()
        {
            mediaFiles = await mediaFileEndpoint.GetAllAsync();
            genres = await genreEndpoint.GetAllAsync();
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
            searchText = await secureStorage.GetAsync(nameof(searchText)) ?? "";
        }

        private async Task SaveFilterState()
        {
            if (string.IsNullOrWhiteSpace(searchText)is false)
            {
                await secureStorage.SetAsync(nameof(searchText), searchText);
            }
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

        private void LoadGenreMedia(GenreModel genre)
        {
            navManager.NavigateTo($"/Genre/{genre.GenreName}");
        }

        private string GetSelectedGenreClass(string genre)
        {
            if (selectedGenre == genre)
            {
                return "bg-white";
            }

            return "";
        }

        private async Task OnSearchInput(string searchInput)
        {
            searchText = searchInput;
            await SaveFilterState();
        }

        private List<MediaFileModel> GetGenreMedias(GenreModel genre)
        {
            var random = new Random();
            return mediaFiles.Where(m => m.Genre?.Id == genre.Id).OrderBy(m => random.Next()).Take(3).ToList();
        }
    }
}