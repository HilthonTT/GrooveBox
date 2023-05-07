namespace GrooveBoxDesktop.Helpers;

public class SecureStorageWrapper : ISecureStorageWrapper
{
    public Task<string> GetAsync(string key)
    {
        return SecureStorage.GetAsync(key);
    }

    public Task SetAsync(string key, string value)
    {
        return SecureStorage.SetAsync(key, value);
    }
}
