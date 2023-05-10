using GrooveBoxLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveBoxApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ConnectionStringController : ControllerBase
{
    private readonly IConfiguration _config;

    public ConnectionStringController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public ConnectionModel GetConnectionStrings()
    {
        string connectionString = _config.GetConnectionString("MongoDB");
        string databaseName = _config.GetValue<string>("MongoDB:DatabaseName");

        ConnectionModel c = new()
        {
            ConnectionString = connectionString,
            DatabaseName = databaseName,
        };

        return c;
    }
}
