using GrooveBoxLibrary.Models;

namespace GrooveBoxApi.DataAccess;

public class ApplicationUserModel
{
    public string Id { get; set; }
    public string ObjectIdentifier { get; set; }
    public string FileName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public Dictionary<string, string> Roles { get; set; } = new();
    public List<BasicMediaFileModel> AuthoredFiles { get; set; } = new();
    public List<BasicMediaFileModel> VotedOnFiles { get; set; } = new();
    public HashSet<string> UserSubscriptions { get; set; } = new();
    public List<BasicUserModel> SubscribedAuthors { get; set; } = new();
}
