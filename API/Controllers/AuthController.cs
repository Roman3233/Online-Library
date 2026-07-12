using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using API.DTOs;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto req)
    {
        var emailExists = await _context.Users.AnyAsync(x => x.Email == req.Email);
        var usernameExists = await _context.Users.AnyAsync(x => x.Username == req.Username);

        if(emailExists || usernameExists) return BadRequest("Email or Username already Exists");

        var user = new User
        {
            Email = req.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Username = req.Username,
            Role = "user"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto req)
    {
        var user_ = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email);
        if(user_ ==null) return Unauthorized("Invalid Email or Password");

        if(!BCrypt.Net.BCrypt.Verify(req.Password, user_.Password))
            return Unauthorized("Invalid Email or Password");

        var token = GenerateJwt(user_);
        return Ok(new AuthResponseDto { Token = token });
    }

    private string GenerateJwt(User user)
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}