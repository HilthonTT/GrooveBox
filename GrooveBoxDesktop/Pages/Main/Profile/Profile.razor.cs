using Microsoft.AspNetCore.Components;
using GrooveBoxLibrary.Models;

namespace GrooveBoxDesktop.Pages.Main.Profile;

public partial class Profile
{
    [Parameter]
    public string ObjectId { get; set; }

    private bool isBusy = false;
    private string sourcePath;
    private UserModel user;
    private List<MediaFileModel> mediaFiles;
    protected override async Task OnInitializedAsync()
    {
        user = await userEndpoint.GetByObjectIdAsync(ObjectId);
        if (user is not null)
        {
            mediaFiles = await mediaFileEndpoint.GetUserMediaFilesAsync(user.Id);
            sourcePath = await fileStorage.CreateSourcePath(user.FileName);
        }
    }

    private async Task Subscribe()
    {
        if (isBusy || user is null)
        {
            return;
        }

        isBusy = true;
        if (user.UserSubscriptions.Add(loggedInUser.Id)is false)
        {
            user.UserSubscriptions.Remove(loggedInUser.Id);
        }

        await userEndpoint.UpdateUserSubscriptionAsync(user.Id, loggedInUser.Id);
        isBusy = false;
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
    }

    private void LoadEditProfilePage()
    {
        navManager.NavigateTo("/EditProfile");
    }

    private string GetSubscribeColor()
    {
        if (user is not null && user.UserSubscriptions.Contains(loggedInUser.Id))
        {
            return "text-danger";
        }

        return "text-success";
    }
}