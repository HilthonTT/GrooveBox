using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Shared;

public partial class NavMenu
{
    private bool showAllAuthors = false;
    private List<GenreModel> genres;
    private string sourcePath = "";
    protected override async Task OnInitializedAsync()
    {
        genres = await genreEndpoint.GetAllAsync();
        if (string.IsNullOrWhiteSpace(loggedInUser.FileName)is false)
        {
            sourcePath = await fileStorage.CreateSourcePath(loggedInUser.FileName);
        }
    }

    private List<BasicUserModel> GetVisibleAuthors()
    {
        if (showAllAuthors)
        {
            return loggedInUser.SubscribedAuthors;
        }
        else
        {
            return loggedInUser.SubscribedAuthors.Take(3).ToList();
        }
    }

    private void ShowAllAuthors()
    {
        showAllAuthors = true;
    }

    private string GetUserSubscriptionCount()
    {
        return loggedInUser.SubscribedAuthors.Count.ToString();
    }

    private void LoadGenreMedia(GenreModel genre)
    {
        navManager.NavigateTo($"/Genre/{genre.GenreName}");
    }

    private void LoadAuthorProfilePage(BasicUserModel author)
    {
        navManager.NavigateTo($"/Profile/{author.ObjectIdentifier}");
    }

    private string GetGenreIconClass(GenreModel genre)
    {
        switch (genre.GenreName)
        {
            case "Music":
                return "oi-musical-note";
            case "Gaming":
                return "oi-bug";
            case "News":
                return "oi-paperclip";
            case "Sports":
                return "oi-badge";
            default:
                return "oi-question-mark";
        }
    }
}