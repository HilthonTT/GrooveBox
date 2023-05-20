using GrooveBoxLibrary.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GrooveBoxLibrary.Models;
public class CreateUserModel
{
    public string ObjectIdentifier { get; set; }
    [Required]
    [NoSpace(ErrorMessage = "No spaces are allowed for first name.")]
    public string FirstName { get; set; }
    [Required]
    [NoSpace(ErrorMessage = "No spaces are allowed for last name.")]
    public string LastName { get; set; }
    [Required]
    [NoSpace(ErrorMessage = "No spaces are allowed for display name.")]
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