using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class UpdateBookDto {
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    public required string Title { get; set; }
}