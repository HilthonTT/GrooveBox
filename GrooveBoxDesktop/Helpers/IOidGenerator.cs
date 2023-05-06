namespace GrooveBoxDesktop.Helpers;

public interface IOidGenerator
{
    Task<string> GenerateOidAsync();
}