using Blazored.LocalStorage;
using GrooveBoxDesktop.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http.Headers;

namespace GrooveBoxDesktop.Authentication;
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _client;
    private readonly AuthenticationStateProvider _authProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _config;
    private string authTokenStorageKey;
    public AuthenticationService(HttpClient client,
                                 AuthenticationStateProvider authProvider,
                                 ILocalStorageService localStorage,
                                 IConfiguration config)
    {
        _client = client;
        _authProvider = authProvider;
        _localStorage = localStorage;
        _config = config;
        authTokenStorageKey = _config["authTokenStorageKey"];
    }

    public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", userForAuthentication.Email),
            new KeyValuePair<string, string>("password", userForAuthentication.Password)
        });

        string api = _config["api"] + _config["tokenEndpoint"];
        var authResult = await _client.PostAsync(api, data);
        var authContent = await authResult.Content.ReadAsStringAsync();

        if (authResult.IsSuccessStatusCode == false)
        {
            return null;
        }

        var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(authContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await _localStorage.SetItemAsync(authTokenStorageKey, result.Access_Token);

        await ((AuthStateProvider)_authProvider).NotifyUserAuthentication(result.Access_Token);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);

        return result;
    }

    public async Task LogOut()
    {
        await ((AuthStateProvider)_authProvider).NotifyUserLogOut();
    }
}
