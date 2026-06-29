using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

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
        return Ok(await _context.Comments.Include(c=> c.User).Include(c => c.Book).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.Id == id);

        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment is null) return NotFound();
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return NoContent();
    }    
}
