using GrooveBoxLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GrooveBoxApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ConnectionStringController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public ConnectionStringController(IConfiguration config, IMemoryCache cache)
    {
        _config = config;
        _cache = cache;
    }

    [HttpGet]
    public DbConnectionModel GetConnectionStrings()
    {
        string connectionString = _cache.Get<string>(nameof(connectionString));
        if (connectionString is null)
        {
            connectionString = _config.GetConnectionString("MongoDB");
            _cache.Set(nameof(connectionString), connectionString, TimeSpan.FromHours(5));
        }

        string databaseName = _cache.Get<string>(nameof(databaseName));
        if (databaseName is null)
        {
            databaseName = _config.GetValue<string>("MongoDB:DatabaseName");
            _cache.Set(nameof(databaseName), databaseName, TimeSpan.FromHours(5));
        }

        DbConnectionModel c = new()
        {
            ConnectionString = connectionString,
            DatabaseName = databaseName,
        };

        return c;
    }
}
