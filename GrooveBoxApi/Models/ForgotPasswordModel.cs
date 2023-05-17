using System.ComponentModel.DataAnnotations;

namespace GrooveBoxApi.Models;

public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }
}
