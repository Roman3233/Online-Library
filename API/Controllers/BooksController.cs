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
    public async Task<IActionResult> GetAll()
    {
        var books = await _context.Books.Include(b => b.User).ToListAsync();
        return Ok(books.Select(b => new BookSummaryDto {
            Id = b.Id,
            Title = b.Title,
            UploadedAt = b.UploadedAt,
            UserId = b.UserId
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _context.Books.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) throw new NotFoundException("Book not found");
        return Ok(new BookSummaryDto {
            Id = book.Id,
            Title = book.Title,
            UploadedAt = book.UploadedAt,
            UserId = book.UserId
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var book = new Book
        {
            Title = dto.Title,
            UserId = userId,
            UploadedAt = DateTime.UtcNow
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
    [HttpPost("{id}/upload")]
    public async Task<IActionResult> Upload(int id, [FromForm] UploadBookDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var existingBook = await _context.Books.FindAsync(id);

        if(existingBook is null) throw new NotFoundException("Book not found");
        if (existingBook.UserId != userId && !User.IsInRole("admin")) 
        throw new ForbiddenException("You don't have permission to upload to this book");
        
        if(dto.File == null || dto.File.Length == 0) throw new ValidationException("File is required");

        string extension = Path.GetExtension(dto.File.FileName);

        if(extension != ".pdf" && extension != ".epub") throw new ValidationException("File type not supported");

        string fileName = Guid.NewGuid().ToString() + extension;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books", fileName);

        if(!Directory.Exists(Path.GetDirectoryName(filePath)))
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        existingBook.FileName = existingBook.Title + extension;
        existingBook.FilePath = fileName;
        existingBook.FileSize = dto.File.Length;
        existingBook.ContentType = dto.File.ContentType;

        await _context.SaveChangesAsync();
        return Ok(existingBook);
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
            UserId = b.UserId
        }));
    }
}