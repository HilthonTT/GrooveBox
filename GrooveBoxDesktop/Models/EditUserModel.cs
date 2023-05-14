using System.ComponentModel.DataAnnotations;

namespace GrooveBoxDesktop.Models;
public class EditUserModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string DisplayName { get; set; }
}
