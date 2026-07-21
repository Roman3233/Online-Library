namespace API.Data;
using Microsoft.EntityFrameworkCore;
using API.Models;


public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate();

        if(context.Users.Any()) return;
        
        var admin = new User
        {
            Email = "admin@mail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "admin",
            Username = "admin"
        };

        var user1 = new User
        {
            Email = "user1@mail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "user",
            Username = "user1"
        };

        var user2 = new User
        {
            Email = "user2@mail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "user",
            Username = "user2"
        };

        context.Users.AddRange(admin, user1, user2);
        context.SaveChanges();

        var (file1Path, file1Size) = SeedBookFile("Clean Code.pdf");
        var book1 = new Book
        {
            Title = "Clean Code",
            UserId = user1.Id,
            FileName = "Clean Code.pdf",
            FilePath = file1Path,
            FileSize = file1Size,
            ContentType = "application/pdf",
            UploadedAt = DateTime.UtcNow,
            Description = "Clean Code: A Handbook of Agile Software Craftsmanship",
            Author = "Robert C. Martin"
        };

        var(file2Path, file2Size) = SeedBookFile("Dark Tower 1.pdf");
        var book2 = new Book
        {
            Title = "Dark Tower 1",
            UserId = user1.Id,
            FileName = "Dark Tower 1.pdf",
            FilePath = file2Path,
            FileSize = file2Size,
            ContentType = "application/pdf",
            UploadedAt = DateTime.UtcNow,
            Description = "Dark Tower 1",
            Author = "Stephen King"
        };

        var(file3Path, file3Size) = SeedBookFile("The Selfish Gene.pdf");
        var book3 = new Book
        {
            Title = "The Selfish Gene",
            UserId = user2.Id,
            FileName = "The Selfish Gene.pdf",
            FilePath = file3Path,
            FileSize = file3Size,
            ContentType = "application/pdf",
            UploadedAt = DateTime.UtcNow,
            Description = "The Selfish Gene",
            Author = "Richard Dawkins"
        };

        context.Books.AddRange(book1, book2, book3);
        context.SaveChanges();

        var comment1 = new Comment
        {
            BookId = book1.Id,
            UserId = user1.Id,
            Text = "Great book!",
            CreatedAt = DateTime.UtcNow
        };

        var comment2 = new Comment
        {
            BookId = book1.Id,
            UserId = user2.Id,
            Text = "I agree!",
            CreatedAt = DateTime.UtcNow
        };

        var comment3 = new Comment
        {
            BookId = book2.Id,
            UserId = user1.Id,
            Text = "Interesting read!",
            CreatedAt = DateTime.UtcNow
        };

        context.Comments.AddRange(comment1, comment2, comment3);
        context.SaveChanges();
    }

    private static (string uniqueFileName, long fileSize) SeedBookFile(string fileName)
    {
        var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedFiles", fileName);
        if (!System.IO.File.Exists(sourcePath))
        {
            return (string.Empty, 0); 
        }

        var uniqueFileName = Guid.NewGuid().ToString() + ".pdf";
        var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books", uniqueFileName);

        var destDir = Path.GetDirectoryName(destinationPath);
        if (destDir != null && !Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

        System.IO.File.Copy(sourcePath, destinationPath, overwrite: true);

        var fileSize = new FileInfo(destinationPath).Length;

        return (uniqueFileName, fileSize);
    }
    
    
}
