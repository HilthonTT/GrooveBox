namespace GrooveBoxDesktop.Pages.Authentication;

public partial class Logout
{
    protected override async Task OnInitializedAsync()
    {
        await authService.LogOut();
        navManager.NavigateTo("/");
    }
}