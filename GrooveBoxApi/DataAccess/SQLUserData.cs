﻿using GrooveBoxApi.Models;

namespace GrooveBoxApi.DataAccess;

public class SQLUserData : ISQLUserData
{
    private readonly ISqlDataAccess _sql;
    private const string DataBaseName = "GrooveBoxData";

    public SQLUserData(ISqlDataAccess sql)
    {
        _sql = sql;
    }

    public SQLUserModel GetUserById(string id)
    {
        var output = _sql.LoadData<SQLUserModel, dynamic>("dbo.spUser_GetById", new { id }, DataBaseName);

        return output.FirstOrDefault();
    }

    public SQLUserModel GetUserByObjectId(string objectId)
    {
        var output = _sql.LoadData<SQLUserModel, dynamic>("dbo.spUser_GetByObjectId", new { objectId }, DataBaseName);

        return output.FirstOrDefault();
    }

    public void InsertUser(SQLUserModel user)
    {
        _sql.SaveData("dbo.spUser_Insert", user, DataBaseName);
    }

    public void UpdateUser(SQLUserModel user)
    {
        _sql.SaveData("dbo.spUser_Update", user, DataBaseName);
    }
}
