namespace GrooveBoxApiLibrary.MongoDataAccess;
public interface IGenreData
{
    Task CreateGenreAsync(GenreModel genre);
    Task<List<GenreModel>> GetAllGenresAsync();
}