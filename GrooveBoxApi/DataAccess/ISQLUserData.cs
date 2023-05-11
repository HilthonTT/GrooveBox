using GrooveBoxApi.Models;

namespace GrooveBoxApi.DataAccess;
public interface ISQLUserData
{
    SQLUserModel GetUserById(string id);
    SQLUserModel GetUserByObjectId(string objectId);
    void InsertUser(SQLUserModel user);
    void UpdateUser(SQLUserModel user);
}