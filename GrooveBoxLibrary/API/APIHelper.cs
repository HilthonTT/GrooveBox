using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace GrooveBoxLibrary.API;
public class APIHelper : IAPIHelper
{
    private readonly ILoggedInUserModel _loggedInUser;
    private readonly IConfiguration _config;
    private readonly IUserData _userData;
    private HttpClient _apiClient;

    public APIHelper(ILoggedInUserModel loggedInUser,
                     IConfiguration config,
                     IUserData userData)
    {
        _loggedInUser = loggedInUser;
        _config = config;
        _userData = userData;
        InitializeClient();
    }

    public HttpClient ApiClient
    {
        get
        {
            return _apiClient;
        }
    }

    private void InitializeClient()
    {
        string api = _config["api"];

        _apiClient = new();
        _apiClient.BaseAddress = new Uri(api);
        _apiClient.DefaultRequestHeaders.Accept.Clear();
        _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<AuthenticatedUser> Authenticate(string username, string password)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string ,string>("password", password)
        });

        using HttpResponseMessage response = await _apiClient.PostAsync("/Token", data);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public void LogOffUser()
    {
        _apiClient.DefaultRequestHeaders.Clear();
    }

    public async Task GetLoggedInUserInfoAsync(string token)
    {
        _apiClient.DefaultRequestHeaders.Clear();
        _apiClient.DefaultRequestHeaders.Accept.Clear();
        _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        using HttpResponseMessage response = await _apiClient.GetAsync("/api/user/GetByObjectId");
        if (response.IsSuccessStatusCode)
        {
            var responseResult = await response.Content.ReadAsAsync<LoggedInUserModel>();
            var dbResult = await _userData.GetUserFromAuthenticationAsync(responseResult.ObjectIdentifier);

            _loggedInUser.Id = dbResult.Id;
            _loggedInUser.ObjectIdentifier = responseResult.ObjectIdentifier;
            _loggedInUser.FileName = dbResult.FileName;
            _loggedInUser.FirstName = dbResult.FirstName;
            _loggedInUser.LastName = dbResult.LastName;
            _loggedInUser.DisplayName = responseResult.DisplayName;
            _loggedInUser.EmailAddress = dbResult.EmailAddress;
            _loggedInUser.Roles = responseResult.Roles;
            _loggedInUser.AuthoredFiles = dbResult.AuthoredFiles;
            _loggedInUser.VotedOnFiles = dbResult.VotedOnFiles;
            _loggedInUser.Token = token;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
