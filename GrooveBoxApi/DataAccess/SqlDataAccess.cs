using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GrooveBoxApi.DataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    private string GetConnectionString(string name)
    {
        return _config.GetConnectionString(name);
    }

    public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using IDbConnection connection = new SqlConnection(connectionString);
        List<T> rows = connection.Query<T>(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure).ToList();

        return rows;
    }

    public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using IDbConnection connection = new SqlConnection(connectionString);
        connection.Execute(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure);
    }
}
