namespace API.DTOs;

public class BookSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? Username { get; set; }
    public int UserId { get; set; }
}