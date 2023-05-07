namespace GrooveBoxLibrary.DataAccess;

public interface IGenreData
{
    Task CreateGenreAsync(GenreModel genre);
    Task<List<GenreModel>> GetAllGenresAsync();
}