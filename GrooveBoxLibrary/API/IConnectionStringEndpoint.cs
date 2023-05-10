namespace GrooveBoxLibrary.API;

public interface IConnectionStringEndpoint
{
    Task<ConnectionModel> GetConnectionStrings();
}