using Microsoft.AspNetCore.Identity;

namespace GrooveBoxApi.DataBaseModel;

public class ApplicationUser : IdentityUser
{
    public string ObjectIdentifier { get; set; }
}
