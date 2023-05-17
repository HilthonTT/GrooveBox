namespace GrooveBoxLibrary.API;

public interface IConnectionStringEndpoint
{
    Task<DbConnectionModel> GetConnectionStrings();
}