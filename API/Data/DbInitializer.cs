namespace API.Data;
using Microsoft.EntityFrameworkCore;
using API.Models;


public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate();

        if(context.Users.Any()) return;
        
        var admin = new User
        {
            Email = "admin@mail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "admin",
            Username = "admin"
        };

        var user1 = new User
        {
            Email = "user1@mail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "user",
            Username = "user1"
        };

        var user2 = new User
        {
            Email = "user2@mail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "user",
            Username = "user2"
        };


        context.Users.AddRange(admin, user1, user2);
        context.SaveChanges();
    }
}
