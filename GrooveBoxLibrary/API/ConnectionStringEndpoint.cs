namespace GrooveBoxLibrary.API;
public class ConnectionStringEndpoint : IConnectionStringEndpoint
{
    private readonly IAPIHelper _apiHelper;

    public ConnectionStringEndpoint(IAPIHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task<DbConnectionModel> GetConnectionStrings()
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/ConnectionString");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<DbConnectionModel>();
            return result;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
