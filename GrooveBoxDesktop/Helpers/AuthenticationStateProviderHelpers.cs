using GrooveBoxLibrary.API;
using GrooveBoxLibrary.DataAccess;
using GrooveBoxLibrary.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GrooveBoxDesktop.Helpers;
public static class AuthenticationStateProviderHelpers
{
    public static async Task<UserModel> GetUserFromAuth(this AuthenticationStateProvider provider,
                                                        IUserData userData,
                                                        IUserEndpoint userEndpoint)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        string userId = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await userEndpoint.GetByIdAsync(userId);
        
        return await userData.GetUserFromAuthenticationAsync(user.ObjectIdentifier);
    }
}
