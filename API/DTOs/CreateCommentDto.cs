
namespace API.DTOs;

public class CreateCommentDto {
    public string Text { get; set; } = string.Empty;
    public int BookId { get; set; }
}