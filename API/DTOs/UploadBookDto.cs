using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class UploadBookDto {
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; } = null!;
}