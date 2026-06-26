namespace API.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int UserId  { get; set; }
    public User? User  { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}