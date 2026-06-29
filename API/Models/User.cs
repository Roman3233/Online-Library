namespace API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ICollection<Book> UploadedBooks { get; set; } = new List<Book>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
