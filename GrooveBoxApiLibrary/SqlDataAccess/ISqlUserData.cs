namespace GrooveBoxApiLibrary.SqlDataAccess;

public interface ISqlUserData
{
    Task<SqlUserModel> GetUserById(string id);
    Task<SqlUserModel> GetUserByObjectId(string objectId);
    Task InsertUser(SqlUserModel user);
    Task UpdateUser(SqlUserModel user);
}