using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Comment> Comments { get; set; } = default!;
}