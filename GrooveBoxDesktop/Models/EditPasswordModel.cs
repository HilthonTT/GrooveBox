using System.ComponentModel.DataAnnotations;

namespace GrooveBoxDesktop.Models;
public class EditPasswordModel
{
    [Required]
    public string Password { get; set; }
}
