using GrooveBoxDesktop.Models;

namespace GrooveBoxDesktop.Authentication;
public interface IAuthenticationService
{
    Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication);
    Task LogOut();
}