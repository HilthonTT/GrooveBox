using System.ComponentModel.DataAnnotations;

namespace GrooveBoxApi.Models;

public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [Required]
    public string Password { get; set; }
    public string Token { get; set; }
}
