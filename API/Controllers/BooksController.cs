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

        return Ok(filteredBooks.Select(b => new BookSummaryDto {
            Id = b.Id,
            Title = b.Title,
            UploadedAt = b.UploadedAt,
            Author = b.Author,
            Description = b.Description,
            UserId = b.UserId,
            HasLiked = b.Likes.Any(l => l.UserId == userId),
            LikeCount = b.Likes.Count
        }));
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
        return Ok(new BookSummaryDto {
            Id = book.Id,
            Title = book.Title,
            UploadedAt = book.UploadedAt,
            Author = book.Author,
            Description = book.Description,
            UserId = book.UserId,
            HasLiked = hasLiked,
            LikeCount = likeCount
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateBookDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);
        if (dto.File == null || dto.File.Length == 0) throw new ValidationException("File is required");

        string extension = Path.GetExtension(dto.File.FileName);
        if(extension != ".pdf") throw new ValidationException("File type not supported");

        string fileName = Guid.NewGuid().ToString() + extension;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books", fileName);

        if(!Directory.Exists(Path.GetDirectoryName(filePath)))
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        string coverFileName = "default.jpg";
        string coverFilePath = "default.jpg";
        string coverContentType = "image/jpeg";
        
        if (dto.Cover != null && dto.Cover.Length > 0) {
            coverContentType = Path.GetExtension(dto.Cover.FileName);
            coverFileName = Guid.NewGuid().ToString() + coverContentType;
            coverFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Covers", coverFileName);

            if(!Directory.Exists(Path.GetDirectoryName(coverFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(coverFilePath)!);
            
            using (var stream = new FileStream(coverFilePath, FileMode.Create))
            {
                await dto.Cover.CopyToAsync(stream);
            }
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
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

        await _context.SaveChangesAsync();
        return NoContent();
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
            UserId = b.UserId
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