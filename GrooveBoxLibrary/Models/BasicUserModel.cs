namespace GrooveBoxLibrary.Models;
public class BasicUserModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ObjectIdentifier { get; set; }
    public string DisplayName { get; set; }
    public string FileName { get; set; }

    public BasicUserModel()
    {
        
    }

    public BasicUserModel(UserModel user)
    {
        Id = user.Id;
        ObjectIdentifier = user.ObjectIdentifier;
        DisplayName = user.DisplayName;
        FileName = user.FileName;
    }

    public BasicUserModel(ILoggedInUserModel user)
    {
        Id = user.Id;
        ObjectIdentifier = user.ObjectIdentifier;
        DisplayName = user.DisplayName;
        FileName = user.FileName;
    }
}
