using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<Friend> Friendships { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    
}