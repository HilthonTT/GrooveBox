using System.ComponentModel.DataAnnotations;

namespace GrooveBoxLibrary.Models;
public class CreateUserModel
{
    public string ObjectIdentifier { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string DisplayName { get; set; }
    [Required]
    public string EmailAddress { get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[A-Z]).+$", ErrorMessage = "Password must contain at least one capital letter.")]
    public string Password { get; set; }
    [Required]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; }
    [Display(Name = "Profile picture")]
    public string FileName { get; set; }
}
