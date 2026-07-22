namespace API.Models;

public class BookLike
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}