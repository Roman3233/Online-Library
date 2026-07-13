using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class CreateCommentDto {
    [Required(ErrorMessage = "Text is required")]
    [MaxLength(5000, ErrorMessage = "Text cannot exceed 5000 characters")]
    public required string Text { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "BookId must be a valid positive integer")]
    public int BookId { get; set; }
}