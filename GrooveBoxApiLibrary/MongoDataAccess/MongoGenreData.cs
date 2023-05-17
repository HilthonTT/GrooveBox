namespace GrooveBoxApiLibrary.MongoDataAccess;
public class MongoGenreData : IGenreData
{
    private readonly IMongoCollection<GenreModel> _genres;
    private readonly IMemoryCache _cache;
    private const string CacheName = "GenreData";

    public MongoGenreData(IDbConnection db, IMemoryCache cache)
    {
        _cache = cache;
        _genres = db.GenreCollection;
    }

    public async Task<List<GenreModel>> GetAllGenresAsync()
    {
        var output = _cache.Get<List<GenreModel>>(CacheName);
        if (output is null)
        {
            var results = await _genres.FindAsync(_ => true);
            output = await results.ToListAsync();

            _cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }

        return output;
    }

    public Task CreateGenreAsync(GenreModel genre)
    {
        return _genres.InsertOneAsync(genre);
    }
}
