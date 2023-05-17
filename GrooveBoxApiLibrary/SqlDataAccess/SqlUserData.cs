namespace GrooveBoxApiLibrary.SqlDataAccess;
public class SqlUserData : ISqlUserData
{
    private readonly ISqlDataAccess _sql;
    private const string DbName = "GrooveBoxData";

    public SqlUserData(ISqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<SqlUserModel> GetUserById(string id)
    {
        var data = new { id };
        var output = await _sql.LoadDataAsync<SqlUserModel, dynamic>("dbo.spUser_GetById", data, DbName);

        return output.FirstOrDefault();
    }

    public async Task<SqlUserModel> GetUserByObjectId(string objectId)
    {
        var data = new { objectId };
        var output = await _sql.LoadDataAsync<SqlUserModel, dynamic>("dbo.spUser_GetByObjectId", data, DbName);

        return output.FirstOrDefault();
    }

    public async Task InsertUser(SqlUserModel user)
    {
        await _sql.SaveDataAsync("dbo.spUser_Insert", user, DbName);
    }

    public async Task UpdateUser(SqlUserModel user)
    {
        var data = new { ObjectId = user.ObjectIdentifier, user.FirstName, user.LastName, user.DisplayName, user.EmailAddress };
        await _sql.SaveDataAsync("dbo.spUser_Update", data, DbName);
    }
}
