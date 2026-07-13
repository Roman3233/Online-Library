namespace API.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int UserId  { get; set; }
    public User? User  { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}