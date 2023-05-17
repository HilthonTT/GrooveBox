using System.ComponentModel.DataAnnotations;

namespace GrooveBoxApi.Models;

public class UserRegistrationModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [Display(Name = "Display Name")]
    public string DisplayName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; }

    [Required]
    public string Password { get; set; }

    [Display(Name = "File Name")]
    public string FileName { get; set; }
}
