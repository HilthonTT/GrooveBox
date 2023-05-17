namespace GrooveBoxLibrary.API;
public class ConnectionStringEndpoint : IConnectionStringEndpoint
{
    private readonly IAPIHelper _apiHelper;
    private readonly IMemoryCache _cache;
    private const string CacheName = "DbConnectionString";

    public ConnectionStringEndpoint(IAPIHelper apiHelper,
                                    IMemoryCache cache)
    {
        _apiHelper = apiHelper;
        _cache = cache;
    }

    public async Task<DbConnectionModel> GetConnectionStrings()
    {
        var output = _cache.Get<DbConnectionModel>(CacheName);
        if (output is null)
        {
            using var response = await _apiHelper.ApiClient.GetAsync("api/ConnectionString");
            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsAsync<DbConnectionModel>();
                _cache.Set(CacheName, output, TimeSpan.FromMinutes(30));
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        return output;
    }
}
