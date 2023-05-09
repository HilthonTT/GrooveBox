namespace GrooveBoxLibrary.API;
public class FileEndpoint : IFileEndpoint
{
    private readonly IAPIHelper _apiHelper;

    public FileEndpoint(IAPIHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task<ObjectId> StoreFileAsync(FileUploadModel model)
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("api/file", model);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsAsync<ObjectId>();
            return responseBody;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }

    public async Task<string> GetSourcePathAsync(string fileId)
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync($"api/file/{fileId}");

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        else
        {
            throw new Exception(response.ReasonPhrase);
        }
    }
}
