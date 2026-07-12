using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Data;
using API.Models;

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
        return Ok(await _context.Books.Include(b => b.User).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _context.Books.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
        return book is null ? NotFound() : Ok(book);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Book book)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        book.UserId = userId;
        book.UploadedAt = DateTime.UtcNow;

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return Ok();        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Book book)
    {
        var existingBook = await _context.Books.FindAsync(id);
        if (existingBook is null) return NotFound();

        existingBook.Title = book.Title;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return NotFound();
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}