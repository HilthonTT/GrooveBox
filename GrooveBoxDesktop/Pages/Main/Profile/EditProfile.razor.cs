using Microsoft.AspNetCore.Components.Forms;
using GrooveBoxLibrary.Models;
using GrooveBoxDesktop.Models;

namespace GrooveBoxDesktop.Pages.Main.Profile;

public partial class EditProfile
{
    private EditUserModel user = new();
    private EditEmailModel email = new();
    private EditPasswordModel password = new();
    private bool isProfileDialogOpen = false;
    private bool isDataDialogOpen = false;
    private bool isEmailDialogOpen = false;
    private bool isPasswordDialogOpen = false;
    private long maxFileSize = 1024 * 1024 * 10; // represents 5MB
    private IBrowserFile file;
    private string sourcePath;
    protected override async Task OnInitializedAsync()
    {
        sourcePath = await fileStorage.CreateSourcePath(loggedInUser?.FileName);
        user.FirstName = loggedInUser.FirstName;
        user.LastName = loggedInUser.LastName;
        user.DisplayName = loggedInUser.DisplayName;
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
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

    private async Task UpdateData()
    {
        loggedInUser.FirstName = user.FirstName;
        loggedInUser.LastName = user.LastName;
        loggedInUser.DisplayName = user.DisplayName;
        var u = MapUser();
        await userEndpoint.UpdateUserAsync(u);
        isDataDialogOpen = false;
    }

    private async Task UpdateEmailRequest()
    {
        loggedInUser.EmailAddress = email.NewEmail;
        await userEndpoint.ResetEmailAsync(loggedInUser.ObjectIdentifier, loggedInUser.EmailAddress);
        isEmailDialogOpen = false;
    }

    private async Task UpdatePasswordRequest()
    {
        await userEndpoint.ForgotPasswordAsync(loggedInUser.Token, loggedInUser.EmailAddress, password.Password);
        isPasswordDialogOpen = false;
    }

    private async Task UpdateProfilePicture()
    {
        if (file is null)
        {
            isProfileDialogOpen = false;
            return;
        }

        string fileId = await fileProcessor.CompressAndStoreImageAsync(file);
        var u = MapUser();
        u.FileName = fileId.ToString();
        await userEndpoint.UpdateUserAsync(u);
        file = null;
        isProfileDialogOpen = false;
    }

    private UserModel MapUser()
    {
        return new UserModel(loggedInUser);
    }
}