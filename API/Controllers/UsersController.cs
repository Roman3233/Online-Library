using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Users.Include(u => u.UploadedBooks).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.Include(u => u.UploadedBooks).FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        var emailExists = await _context.Users.AnyAsync(x => x.Email == user.Email);
        if (emailExists) return BadRequest("Email already exists");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] User user)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser is null) return NotFound();

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.Password = user.Password;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}