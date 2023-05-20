using GrooveBoxLibrary.Models;
using GrooveBoxDesktop.Models;

namespace GrooveBoxDesktop.Pages.Authentication;

public partial class Register
{
    CreateUserModel user = new();
    private string errorMessage = "";
    private void LoadLoginPage()
    {
        navManager.NavigateTo("/login");
    }

    private async Task OpenAlertFromService()
    {
        await matDialogService.AlertAsync(errorMessage);
    }

    private async Task RegisterAsync()
    {
        try
        {
            errorMessage = "";
            UserModel u = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress,
                FileName = "",
            };
            await userEndpoint.CreateUserAsync(user);
            ;
            AuthenticatedUserModel result = await authService.Login(new() { Email = user.EmailAddress, Password = user.Password });
            if (result is not null)
            {
                navManager.NavigateTo("/");
            }
            else
            {
                errorMessage = "Registration successfull but login failed.";
                await OpenAlertFromService();
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }
}