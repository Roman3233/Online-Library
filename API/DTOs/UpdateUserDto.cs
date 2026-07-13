using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class UpdateUserDto {
    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
    [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters")]
    public required string Username { get; set; } = string.Empty;
}