using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagUsers> TagUsers { get; set; }
    public DbSet<Friend> Friendships { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        User u1 = new User { 
            Id = 1,
            Username = "Tester1",
            Email = "tester1@gmail.com",
            Password = PasswordGenerator.GeneratePasswordHash("12345678"),
            Points = 0
            
        };

        User u2 = new User
        {
            Id = 2,
            Username = "Tester2",
            Email = "tester2@gmail.com",
            Password = PasswordGenerator.GeneratePasswordHash("12345678"),
            Points = 0

        };

        User u3 = new User
        {
            Id = 3,
            Username = "Tester3",
            Email = "tester3@gmail.com",
            Password = PasswordGenerator.GeneratePasswordHash("12345678"),
            Points = 0

        };

        modelBuilder.Entity<User>().HasData(
            u1,u2,u3
        );
    }
}