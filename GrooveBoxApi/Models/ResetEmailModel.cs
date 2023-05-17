using System.ComponentModel.DataAnnotations;

namespace GrooveBoxApi.Models;

public class ResetEmailModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }
}
