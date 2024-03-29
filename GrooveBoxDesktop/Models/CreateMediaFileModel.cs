﻿using System.ComponentModel.DataAnnotations;

namespace GrooveBoxDesktop.Models;
public class CreateMediaFileModel
{
    [Required(ErrorMessage = "The title is required to upload your media.")]
    [MaxLength(75)]
    public string Title { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Display(Name = "File Path")]
    public string FilePath { get; set; }

    [Required(ErrorMessage = "The genre is required to upload your media.")]
    [MinLength(1)]
    public string GenreId { get; set; }
}