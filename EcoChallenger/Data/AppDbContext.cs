using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

    }

    public DbSet<User> User { get; set; }
    public DbSet<UserToken> UserToken { get; set; }
}