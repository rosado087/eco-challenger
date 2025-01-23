using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}