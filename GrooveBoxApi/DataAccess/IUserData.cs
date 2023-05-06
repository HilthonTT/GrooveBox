using GrooveBoxApi.Models;

namespace GrooveBoxApi.DataAccess;
public interface IUserData
{
    UserModel GetUserById(string id);
    UserModel GetUserByObjectId(string objectId);
    void InsertUser(UserModel user);
    void UpdateUser(UserModel user);
}