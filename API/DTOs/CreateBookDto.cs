using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class CreateBookDto {
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public required string Title { get; set; }
}
