using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagUsers> TagUsers { get; set; }
    public DbSet<Friend> Friendships { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        User u1 = new User
        {
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

        modelBuilder.Entity<User>().HasData(u1, u2, u3);

        Tag t1 = new Tag
        {
            Id = 1,
            Name = "Eco-Warrior"
        };

        Tag t2 = new Tag
        {
            Id = 2,
            Name = "NatureLover"
        };

        Tag t3 = new Tag
        {
            Id = 3,
            Name = "Green Guru"
        };

        modelBuilder.Entity<Tag>().HasData(t1, t2, t3);

        /*TagUsers tu1 = new TagUsers
        {
            Id = 1,
            User = u1,
            Tag = t1,
            SelectedTag = false
        };

        TagUsers tu2 = new TagUsers
        {
            Id = 2,
            User = u1,
            Tag = t2,
            SelectedTag = false
        };

        TagUsers tu3 = new TagUsers
        {
            Id = 3,
            User = u1,
            Tag = t3,
            SelectedTag = false
        };
        TagUsers tu4 = new TagUsers
        {
            Id = 4,
            User = u3,
            Tag = t2,
            SelectedTag = true


        };

        modelBuilder.Entity<TagUsers>().HasData(tu1,tu2,tu3,tu4);*/
    }
}