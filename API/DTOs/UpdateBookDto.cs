using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class UpdateBookDto {
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    public required string Title { get; set; }
    [Required(ErrorMessage = "Author is required")]
    [MaxLength(100, ErrorMessage = "Author cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Author must be at least 3 characters")]
    public required string Author { get; set; }
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(10000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
    public required string Description { get; set; }
    [Required(ErrorMessage = "ImgUrl is required")]
    public required string ImgUrl { get; set; }
}