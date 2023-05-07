namespace GrooveBoxDesktop.Helpers;

internal interface ISecureStorageWrapper
{
    Task<string> GetAsync(string key);
    Task SetAsync(string key, string value);
}