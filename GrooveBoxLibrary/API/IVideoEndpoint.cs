namespace GrooveBoxLibrary.API;

public interface IVideoEndpoint
{
    Task<string> GetVideoUrlAsync(string id);
}