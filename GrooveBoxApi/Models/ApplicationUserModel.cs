namespace GrooveBoxApi.DataAccess;

public class ApplicationUserModel
{
    public string Id { get; set; }
    public string ObjectIdentifier { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public Dictionary<string, string> Roles { get; set; } = new();
}
