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
    public DbSet<Challenge> Challenges { get; set; }
    //public DbSet<WeeklyChallenge> WeeklyChallenges { get; set; }
    public DbSet<UserChallenges> UserChallenges { get; set; }

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


        modelBuilder.Entity<Challenge>().HasData(
        
            new Challenge()
            {
                Id = 1,
                Title = "Daily Challenge 1",
                Description = "Description 1",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 2,
                Title = "Daily Challenge 2",
                Description = "Description 2",
                Points = 10,
                Type = "Daily"
            }
            ,
            new Challenge()
            {
                Id = 3,
                Title = "Daily Challenge 3",
                Description = "Description 3",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 4,
                Title = "Daily Challenge 4",
                Description = "Description 4",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 5,
                Title = "Daily Challenge 5",
                Description = "Description 5",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 6,
                Title = "Daily Challenge 6",
                Description = "Description 6",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 7,
                Title = "Daily Challenge 7",
                Description = "Description 7",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 8,
                Title = "Daily Challenge 8",
                Description = "Description 8",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 9,
                Title = "Daily Challenge 9",
                Description = "Description 9",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 10,
                Title = "Daily Challenge 10",
                Description = "Description 10",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Id = 11,
                Title = "Weekly Challenge 1",
                Description = "Description 1",
                Points = 100,
                MaxProgress = 5,
                Type = "Weekly"
            },
            new Challenge()
            {
                Id = 12,
                Title = "Weekly Challenge 2",
                Description = "Description 2",
                Points = 160,
                MaxProgress = 7,
                Type = "Weekly"
            }, 
            new Challenge()
            {
                Id = 13,
                Title = "Weekly Challenge 3",
                Description = "Description 3",
                Points = 60,
                MaxProgress = 3,
                Type = "Weekly"
            }, 
            new Challenge()
            {
                Id = 14,
                Title = "Weekly Challenge 4",
                Description = "Description 4",
                Points = 100,
                MaxProgress = 5,
                Type = "Weekly"
            }, 
            new Challenge()
            {
                Id = 15,
                Title = "Weekly Challenge 5",
                Description = "Description 5",
                Points = 80,
                MaxProgress = 4,
                Type = "Weekly"
            }
        );

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