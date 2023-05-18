using System.ComponentModel.DataAnnotations;

namespace GrooveBoxDesktop.Models;
public class EditEmailModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "New Email")]
    public string NewEmail { get; set; }
}
