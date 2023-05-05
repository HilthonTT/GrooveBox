using GrooveBoxApi.Models;

namespace GrooveBoxApi.DataAccess;

public class UserData : IUserData
{
    private readonly ISqlDataAccess _sql;
    private const string DataBaseName = "GrooveBoxData";

    public UserData(ISqlDataAccess sql)
    {
        _sql = sql;
    }

    public UserModel GetUserById(string id)
    {
        var output = _sql.LoadData<UserModel, dynamic>("dbo.spUser_GetById", new { id }, DataBaseName);

        return output.FirstOrDefault();
    }

    public void InsertUser(UserModel user)
    {
        _sql.SaveData("dbo.spUser_Insert", user, DataBaseName);
    }

    public void UpdateUser(UserModel user)
    {
        _sql.SaveData("dbo.spUser_Update", user, DataBaseName);
    }
}
