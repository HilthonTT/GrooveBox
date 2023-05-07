namespace GrooveBoxLibrary.Models;

public interface ILoggedInUserModel
{
    List<BasicMediaFileModel> AuthoredFiles { get; set; }
    string DisplayName { get; set; }
    string EmailAddress { get; set; }
    string FileName { get; set; }
    string FirstName { get; set; }
    string Id { get; set; }
    string LastName { get; set; }
    string ObjectIdentifier { get; set; }
    string Token { get; set; }
    List<BasicMediaFileModel> VotedOnFiles { get; set; }
    Dictionary<string, string> Roles { get; set; }
    List<BasicUserModel> SubscribedAuthors { get; set; }
}