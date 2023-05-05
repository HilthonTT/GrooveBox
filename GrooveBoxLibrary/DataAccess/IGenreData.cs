namespace GrooveBoxLibrary.DataAccess;

public interface IGenreData
{
    Task CreateCategoryAsync(GenreModel genre);
    Task<List<GenreModel>> GetAllGenresAsync();
}