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
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;
    public CommentsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comments = await _context.Comments.Include(c=> c.User).Include(c => c.Book).ToListAsync();
        return Ok(comments.Select(c => new CommentSummaryDto {
            Id = c.Id,
            Text = c.Text,
            CreatedAt = c.CreatedAt,
            BookId = c.BookId
        }));
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetBookComments(int bookId)
    {
        var comments = await _context.Comments.Include(c=> c.User).Where(c => c.BookId == bookId).ToListAsync();
        return Ok(comments.Select(c => new CommentSummaryDto {
            Id = c.Id,
            Text = c.Text,
            CreatedAt = c.CreatedAt,
            BookId = c.BookId,
            Username = c.User?.Username
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment is null) throw new NotFoundException("Comment not found");
        return Ok(new CommentSummaryDto {
            Id = comment.Id,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            BookId = comment.BookId
        });
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var comment = new Comment
        {
            Text = dto.Text,
            BookId = dto.BookId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim);
        
        var comment = await _context.Comments.FindAsync(id);
        if (comment is null) throw new NotFoundException("Comment not found");
        if (comment.UserId != userId && !User.IsInRole("admin"))
            throw new ForbiddenException("You don't have permission to delete this comment");
        
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return NoContent();
    }    
}
