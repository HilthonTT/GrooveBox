using Microsoft.AspNetCore.Components;
using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Media;

public partial class MediaDetails
{
    [Parameter]
    public string Id { get; set; }

    private MediaFileModel media;
    private UserModel author;
    private string videoSource;
    private bool isBusy = false;
    protected override async Task OnInitializedAsync()
    {
        media = await mediaFileEndpoint.GetMediaFileAsync(Id);
        videoSource = await GetVideoSource();
        if (media is not null)
        {
            author = await userEndpoint.GetByObjectIdAsync(media.Author.ObjectIdentifier);
        }
    }

    private async Task<string> GetVideoSource()
    {
        string source = await videoEndpoint.GetVideoUrlAsync(media.FilePath);
        if (string.IsNullOrWhiteSpace(source)is false)
        {
            return source;
        }

        return "";
    }

    private async Task Subscribe()
    {
        if (isBusy || author is null || author.Id == loggedInUser.Id)
        {
            return;
        }

        isBusy = true;
        if (author.UserSubscriptions.Add(loggedInUser.Id)is false)
        {
            author.UserSubscriptions.Remove(loggedInUser.Id);
        }

        await userEndpoint.UpdateUserSubscriptionAsync(author.Id, loggedInUser.Id);
        isBusy = false;
    }

    private async Task VoteUp()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;
        if (string.IsNullOrWhiteSpace(loggedInUser.Id))
        {
            navManager.NavigateTo("/Login");
            return;
        }

        if (media.UserVotes.Add(loggedInUser.Id)is false)
        {
            media.UserVotes.Remove(loggedInUser.Id);
        }

        await mediaFileEndpoint.UpdateVoteMediaFileAsync(media.Id, loggedInUser.Id);
        isBusy = false;
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
    }

    private void LoadAuthorPage()
    {
        navManager.NavigateTo($"/Profile/{media.Author.Id}");
    }

    private void LoadEditPage()
    {
        navManager.NavigateTo($"/MediaEdit/{media.Id}");
    }

    private string GetSubscribeColor()
    {
        if (author is not null && author.UserSubscriptions.Contains(loggedInUser.Id))
        {
            return "text-danger";
        }

        return "text-success";
    }

    private string GetVoteColor()
    {
        if (media.UserVotes.Contains(loggedInUser.Id))
        {
            return "text-success";
        }

        return "";
    }
}