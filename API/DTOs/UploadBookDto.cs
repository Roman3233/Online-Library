using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class UploadBookDto {
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; } = null!;
}