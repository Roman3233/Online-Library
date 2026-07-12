using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Data;
using API.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context) { _context = context; }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users.Select(u => new UserResponseDto {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role,
            RegisteredAt = u.RegisteredAt
        }));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();
        return Ok(new UserResponseDto {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            RegisteredAt = user.RegisteredAt
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser is null || existingUser.Id != userId) return NotFound();

        existingUser.Username = dto.Username;

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

        var user = await _context.Users.FindAsync(id);
        if (user is null || user.Id != userId) return NotFound();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}