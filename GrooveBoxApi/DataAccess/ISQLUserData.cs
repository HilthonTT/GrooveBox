using GrooveBoxApi.Models;

namespace GrooveBoxApi.DataAccess;
public interface ISQLUserData
{
    UserModel GetUserById(string id);
    UserModel GetUserByObjectId(string objectId);
    void InsertUser(UserModel user);
    void UpdateUser(UserModel user);
}