using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Middleware.Exceptions;

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
        if (user is null) throw new NotFoundException("User not found");
        return Ok(new UserResponseDto {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            RegisteredAt = user.RegisteredAt
        });
    }

    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);
        var user = await _context.Users.FindAsync(userId);
        if (user is null) throw new NotFoundException("User not found");
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
        if (existingUser is null) throw new NotFoundException("User not found");
        if (existingUser.Id != userId || !User.IsInRole("admin")) 
            throw new ForbiddenException("You don't have permission to update this user");

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
        if (user is null) throw new NotFoundException("User not found");
        if (user.Id != userId && !User.IsInRole("admin"))
            return Forbid();
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}