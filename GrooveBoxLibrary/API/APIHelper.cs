﻿using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace GrooveBoxLibrary.API;
public class APIHelper : IAPIHelper
{
    private readonly ILoggedInUserModel _loggedInUser;
    private readonly IConfiguration _config;
    private HttpClient _apiClient;

    public APIHelper(ILoggedInUserModel loggedInUser,
                     IConfiguration config)
    {
        _loggedInUser = loggedInUser;
        _config = config;
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

        _apiClient = new() { BaseAddress = new Uri(api) };
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

        using var response = await _apiClient.PostAsync("/Token", data);
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

        using var response = await _apiClient.GetAsync("/api/user/GetMyId");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<LoggedInUserModel>();

            _loggedInUser.Id = result.Id;
            _loggedInUser.ObjectIdentifier = result.ObjectIdentifier;
            _loggedInUser.FileName = result.FileName;
            _loggedInUser.FirstName = result.FirstName;
            _loggedInUser.LastName = result.LastName;
            _loggedInUser.DisplayName = result.DisplayName;
            _loggedInUser.EmailAddress = result.EmailAddress;
            _loggedInUser.Roles = result.Roles;
            _loggedInUser.AuthoredFiles = result.AuthoredFiles;
            _loggedInUser.VotedOnFiles = result.VotedOnFiles;
            _loggedInUser.SubscribedAuthors = result.SubscribedAuthors;
            _loggedInUser.UserSubscriptions = result.UserSubscriptions;
            _loggedInUser.Token = token;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
