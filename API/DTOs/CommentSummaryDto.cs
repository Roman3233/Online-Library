namespace API.DTOs;
public class CommentSummaryDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int BookId { get; set; }
    public string? Username { get; set; }
}