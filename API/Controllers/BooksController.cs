using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Data;
using API.Models;
using API.DTOs;
using API.Middleware.Exceptions;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context) { _context = context; }

    private static BookSummaryDto MapBookSummary(Book book, bool hasLiked, int likeCount)
    {
        return new BookSummaryDto
        {
            Id = book.Id,
            Title = book.Title,
            UploadedAt = book.UploadedAt,
            Author = book.Author,
            Description = book.Description,
            UserId = book.UserId,
            HasLiked = hasLiked,
            LikeCount = likeCount,
            CoverUrl = $"http://localhost:5164/Resources/Covers/{book.CoverFilePath}",
            PdfUrl = $"http://localhost:5164/api/books/{book.Id}/file"
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int userId = userIdClaim == null ? 0 : int.Parse(userIdClaim);
        
        IQueryable<Book> books = _context.Books.Include(b => b.User).Include(b => b.Likes);
        if (!string.IsNullOrEmpty(search)) {
            search = search.ToLower();
            books = books.Where(b => b.Title.ToLower().Contains(search) || 
            (b.Author != null && b.Author.ToLower().Contains(search)) || 
            (b.Description != null && b.Description.ToLower().Contains(search)));
        }

        var filteredBooks = await books.ToListAsync();

        return Ok(filteredBooks.Select(b => MapBookSummary(b, b.Likes.Any(l => l.UserId == userId), b.Likes.Count)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int userId = userIdClaim == null ? 0 : int.Parse(userIdClaim);
        
        var hasLiked = userId > 0 && await _context.BookLikes
        .AnyAsync(l => l.BookId == id && l.UserId == userId);
        var likeCount = await _context.BookLikes.Where(l => l.BookId == id).CountAsync();

        var book = await _context.Books.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) throw new NotFoundException("Book not found");
        return Ok(MapBookSummary(book, hasLiked, likeCount));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateBookDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);
        if (dto.File == null || dto.File.Length == 0) throw new ValidationException("File is required");

        string fileName = await SaveFileAsync(dto.File, "Books");

        string coverFileName = "default.jpg";
        string coverFilePath = "default.jpg";
        string coverContentType = "image/jpg";
        
        if (dto.Cover != null && dto.Cover.Length > 0) {
            coverFilePath = await SaveCoverAsync(dto.Cover);
            coverFileName = dto.Cover.FileName;
            coverContentType = dto.Cover.ContentType;
        }
        
        var book = new Book
        {
            Title = dto.Title,
            UserId = userId,
            UploadedAt = DateTime.UtcNow,
            Author = dto.Author,
            Description = dto.Description,
            FileName = fileName,
            FilePath = fileName,
            FileSize = dto.File.Length,
            ContentType = dto.File.ContentType,
            CoverFileName = coverFileName,
            CoverFilePath = coverFilePath,
            CoverContentType = coverContentType
        };
        
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateBookDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var existingBook = await _context.Books.FindAsync(id);
        if(existingBook is null) throw new NotFoundException("Book not found");
        if (existingBook.UserId != userId && !User.IsInRole("admin")) 
        throw new ForbiddenException("You don't have permission to update this book");

        existingBook.Title = dto.Title;
        existingBook.Author = dto.Author;
        existingBook.Description = dto.Description;
        
        if(dto.Cover != null) {
            string coverFileName = await SaveCoverAsync(dto.Cover);
            existingBook.CoverFileName = dto.Cover.FileName;
            existingBook.CoverFilePath = coverFileName;
            existingBook.CoverContentType = dto.Cover.ContentType;
        }
        if(dto.File != null) {
            string fileName = await SaveFileAsync(dto.File, "Books");
            existingBook.FileName = dto.File.FileName;
            existingBook.FilePath = fileName;
            existingBook.FileSize = dto.File.Length;
            existingBook.ContentType = dto.File.ContentType;
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        string extension = Path.GetExtension(file.FileName);
        if(extension != ".pdf") throw new ValidationException("File type not supported");

        string fileName = Guid.NewGuid().ToString() + extension;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", folder, fileName);

        await WriteFileAsync(filePath, file);

        return fileName;
    }

    private async Task<string> SaveCoverAsync(IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName);
        string fileName = Guid.NewGuid().ToString() + extension;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Covers", fileName);

        await WriteFileAsync(filePath, file);

        return fileName;
    }

    private async Task WriteFileAsync(string oldFilePath, IFormFile file)
    {
         if (!Directory.Exists(Path.GetDirectoryName(oldFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(oldFilePath)!);

        using var stream = new FileStream(oldFilePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim);

        var book = await _context.Books.FindAsync(id);
        if (book is null) throw new NotFoundException("Book not found");
        
        if (book.UserId != userId && !User.IsInRole("admin")) 
        throw new ForbiddenException("You don't have permission to delete this book");
        
        string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books", book.FilePath);
        if (System.IO.File.Exists(FilePath)) System.IO.File.Delete(FilePath);
        
        if(book.CoverFilePath != "default.jpg")
        {
            string CoverPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Covers", book.CoverFilePath);
            if (System.IO.File.Exists(CoverPath)) System.IO.File.Delete(CoverPath);
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var existingBook = await _context.Books.FindAsync(id);
        if(existingBook is null) throw new NotFoundException("Book not found");

        if (string.IsNullOrEmpty(existingBook.FilePath))
        throw new ValidationException("Book has no file");

        string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books", existingBook.FilePath);
        if (!System.IO.File.Exists(FilePath)) throw new NotFoundException("File not found on server");
        
        return PhysicalFile(FilePath, existingBook.ContentType, existingBook.FileName);
    }

    [AllowAnonymous]
    [HttpGet("{id}/file")]
    public async Task<IActionResult> GetFile(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) throw new NotFoundException("Book not found");

        if (string.IsNullOrEmpty(book.FilePath))
        throw new ValidationException("Book has no file");

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books", book.FilePath);
        if (!System.IO.File.Exists(filePath)) throw new NotFoundException("File not found on server");

        return PhysicalFile(filePath, book.ContentType, enableRangeProcessing: true);
    }
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var books = await _context.Books
            .Where(b => b.UserId == userId)
            .ToListAsync();

        return Ok(books.Select(b => new BookSummaryDto {
            Id = b.Id,
            Title = b.Title,
            UploadedAt = b.UploadedAt,
            Author = b.Author,
            Description = b.Description,
            UserId = b.UserId,
            CoverUrl = $"http://localhost:5164/Resources/Covers/{b.CoverFilePath}",
            PdfUrl = $"http://localhost:5164/api/books/{b.Id}/file"
        }));
    }

    [Authorize]
    [HttpPost("{id}/like")]
    public async Task<IActionResult> Like(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var existingBook = await _context.Books.FindAsync(id);
        
        if(existingBook is null) throw new NotFoundException("Book not found");
        
        bool hasLiked = await _context.BookLikes.AnyAsync(l => l.BookId == id && l.UserId == userId);
        if (hasLiked)
        {
            _context.BookLikes.Remove(new BookLike { BookId = id, UserId = userId });
            hasLiked = false;
        }
        else
        {
            _context.BookLikes.Add(new BookLike { BookId = id, UserId = userId });
            hasLiked = true;
        }
        await _context.SaveChangesAsync();

        var likeCount = await _context.BookLikes.Where(l => l.BookId == id).CountAsync();
        return Ok(new { hasLiked, likeCount });         
    }         
}
