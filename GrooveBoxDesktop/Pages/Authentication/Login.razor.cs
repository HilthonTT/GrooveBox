using GrooveBoxDesktop.Models;

namespace GrooveBoxDesktop.Pages.Authentication;

public partial class Login
{
    private AuthenticationUserModel user = new();
    private string errorMessage = "";
    private async Task ExecuteLogin()
    {
        errorMessage = "";
        AuthenticatedUserModel result = await authService.Login(user);
        if (result is not null)
        {
            navManager.NavigateTo("/");
        }
        else
        {
            errorMessage = "Make sure your credentials are correct.";
            await OpenAlertFromService();
        }
    }

    private async Task OpenAlertFromService()
    {
        await matDialogService.AlertAsync(errorMessage);
    }

    private void LoadRegisterPage()
    {
        navManager.NavigateTo("/register");
    }
}