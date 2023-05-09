namespace GrooveBoxLibrary.API;

public interface IGenreEndpoint
{
    Task CreateGenreAsync(GenreModel genre);
    Task<List<GenreModel>> GetAllAsync();
}