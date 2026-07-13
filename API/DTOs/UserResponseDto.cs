namespace API.DTOs;

public class UserResponseDto {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public ICollection<BookSummaryDto> UploadedBooks { get; set; } = new List<BookSummaryDto>();
    public ICollection<CommentSummaryDto> Comments { get; set; } = new List<CommentSummaryDto>();
    public DateTime RegisteredAt { get; set; }
}