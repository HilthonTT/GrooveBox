using Microsoft.JSInterop;
using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Media
{
    public partial class Subscriptions
    {
        private List<MediaFileModel> medias;
        private List<MediaFileModel> todaysMedias;
        private List<MediaFileModel> thisWeekMedias;
        private List<MediaFileModel> thisMonthsMedias;
        private List<MediaFileModel> thisYearsMedias;
        private string searchText = "";
        protected override async Task OnInitializedAsync()
        {
            medias = await mediaFileEndpoint.GetAllAsync();
            var subscribedAuthorIds = loggedInUser.SubscribedAuthors.Select(a => a.Id).ToList();
            medias = medias.Where(m => subscribedAuthorIds.Contains(m.Author.Id)).ToList();

            todaysMedias = GetMediasCreatedToday();
            thisWeekMedias = GetMediasCreatedThisWeek();
            thisMonthsMedias = GetMediasCreatedThisMonth();
            thisYearsMedias = GetMediasCreatedThisYear();
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

        private async Task OnSearchInput(string searchInput)
        {
            searchText = searchInput;
            await SaveFilterState();
        }

        private List<MediaFileModel> GetMediasCreatedToday()
        {
            var today = DateTime.Today;
            return medias.Where(m => m.DateCreated.Date == today).ToList();
        }

        private List<MediaFileModel> GetMediasCreatedThisWeek()
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);
            return medias.Where(m => m.DateCreated >= startOfWeek && m.DateCreated <= endOfWeek).ToList();
        }

        private List<MediaFileModel> GetMediasCreatedThisMonth()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);
            return medias.Where(m => m.DateCreated >= startOfMonth && m.DateCreated <= endOfMonth).ToList();
        }

        private List<MediaFileModel> GetMediasCreatedThisYear()
        {
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            var endOfYear = startOfYear.AddYears(1).AddSeconds(-1);
            return medias.Where(m => m.DateCreated >= startOfYear && m.DateCreated <= endOfYear).ToList();
        }

        private void LoadResults()
        {
            if (string.IsNullOrWhiteSpace(searchText)is false)
            {
                navManager.NavigateTo($"/results/{searchText}");
            }
            else
            {
                navManager.NavigateTo("/");
            }
        }

        private void ClosePage()
        {
            navManager.NavigateTo("/");
        }
    }
}