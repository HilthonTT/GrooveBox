namespace GrooveBoxLibrary.API;

public interface IAPIHelper
{
    HttpClient ApiClient { get; }

    Task<AuthenticatedUser> Authenticate(string username, string password);
    Task GetLoggedInUserInfoAsync(string token);
    void LogOffUser();
}