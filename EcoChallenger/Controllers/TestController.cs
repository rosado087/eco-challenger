#if DEBUG
using System.Reflection;
using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 
namespace EcoChallenger.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _ctx;
        private readonly ILogger<TestController> _logger;

        public TestController(IWebHostEnvironment env, AppDbContext context, ILogger<TestController> logger) {
            _ctx = context;
            _environment = env;
            _logger = logger;

            if(!_environment.IsEnvironment("Test"))
                throw new Exception("Application must be running in test mode");
        }

        /// <summary>
        /// Method just used in automation testing to retrieve recovery tokens
        /// </summary>
        /// <returns>List of recovery tokens</returns>
        [AllowAnonymous]
        [HttpGet("get-recovery-tokens")]
        public IActionResult getRecovertTokens() {                       
            return Ok(FakeEmailService.Tokens);
        }

        /// <summary>
        /// Resets the database data, this is used in between the tests.
        /// The method uses Reflection to find all the entities!!
        /// </summary>
        /// <returns>List of recovery tokens</returns>
        [AllowAnonymous]
        [HttpPost("reset-db")]
        public IActionResult ResetDatabase()
        {
            // Get all the entity types that are used in DB
            var entityTypes = _ctx.Model.GetEntityTypes();
            
            foreach (var entityType in entityTypes)
            {
                var clrType = entityType.ClrType;

                /*
                * Here we try to fetch the Set method from AppDbContext, but since it has
                * multiple overloads, we must use this contraption to make sure we filter and get
                * the one we want.
                */
                var method = typeof(DbContext)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0)
                    .First();

                var genericMethod = method.MakeGenericMethod(clrType);
                var dbSet = genericMethod.Invoke(_ctx, null);

                var removeRangeMethod = dbSet!.GetType().GetMethod("RemoveRange", new[] { typeof(IEnumerable<>).MakeGenericType(clrType) });

                if (dbSet is IQueryable queryable && removeRangeMethod != null)
                {
                    var toListMethod = typeof(Enumerable)
                        .GetMethod("ToList", BindingFlags.Public | BindingFlags.Static)?
                        .MakeGenericMethod(clrType);

                    if(toListMethod == null) throw new NullReferenceException("Could not get AppDbContext toList method");

                    var entities = toListMethod.Invoke(null, new object[] { queryable });

                    removeRangeMethod.Invoke(dbSet, new[] { entities });
                }
            }

            _ctx.SaveChanges();
            return Ok();
        }
        
        /// <summary>
        /// Fills the database with the default test data.
        /// WARNING: BEWARE THAT CHANGES HERE MAY AFFECT TESTS
        /// </summary>
        [AllowAnonymous]
        [HttpPost("seed-db")]
        public IActionResult SeedDatabase() {
            try {
                // Add test users
                List<User> users = new List<User>() {
                    new User { Username = "testuser", Email = "testuser@example.com", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!"),
                        Points = 300 },

                    new User { Username = "testuserpasswordrecover", 
                        Email = "testuserpasswordrecover@example.com", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!") },

                    new User { Username = "Tester1", Email = "tester1@gmail.com", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!"), 
                        IsAdmin = true, Points = 300 },

                    new User { Username = "Tester2", Email = "tester2@gmail.com", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!"), 
                        Points = 100 },

                    new User { Username = "Tester3", Email = "tester3@gmail.com", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!"), 
                        Points = 100 },

                    new User { Username = "tester4", Email = "201902087@estudantes.ips.pt", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!"), 
                        IsAdmin = true},

                    new User { Username = "olduser", Email = "old@example.com", 
                        Password = PasswordGenerator.GeneratePasswordHash("Password123!"), 
                        Points = 100 }
                };
                _ctx.Users.AddRange(users);
                _ctx.SaveChanges();

                // Create Tags
                List<Tag> tags = new List<Tag>() {
                    new Tag { Name = "Eco-Warrior", Price = 40, BackgroundColor = "#355735", TextColor = "#FFFFFF"},
                    new Tag { Name = "NatureLover", Price = 50, BackgroundColor = "#355735", TextColor = "#FFFFFF"},
                    new Tag { Name ="Green Guru", Price = 30, BackgroundColor = "#355735", TextColor = "#FFFFFF"},
                };
                _ctx.Tags.AddRange(tags);
                _ctx.SaveChanges();

                // Create TagUsers
                _ctx.TagUsers.AddRange(new List<TagUsers>() {
                    new TagUsers() {
                        User = users[2],
                        Tag = tags[0],
                        SelectedTag = true
                    },
                    new TagUsers() {
                        User = users[2],
                        Tag = tags[1],
                        SelectedTag = false
                    },
                    new TagUsers() {
                        User = users[0],
                        Tag = tags[2],
                        SelectedTag = true
                    },
                    new TagUsers() {
                        User = users[0],
                        Tag = tags[0],
                        SelectedTag = false
                    }
                });
                _ctx.SaveChanges();

                // Add Friends
                _ctx.Friendships.AddRange(new List<Friend>() {
                    new Friend() { UserId = users[2].Id, FriendId = users[0].Id },
                    new Friend() { UserId = users[2].Id, FriendId = users[5].Id }
                });
                _ctx.SaveChanges();

                // Add Challenges
                List<Challenge> challenges = new List<Challenge>() {
                    new Challenge { Title = "testChallenge1", Description = "descricao1", Points = 20, Type = "Daily"},
                    new Challenge { Title = "testChallenge2", Description = "descricao2", Points = 20, Type = "Daily"},
                    new Challenge { Title = "testChallenge3", Description = "descricao3", Points = 20, Type = "Daily"},
                    new Challenge { Title = "testChallenge4", Description = "descricao4", Points = 50, Type = "Weekly", MaxProgress = 3},
                    new Challenge { Title = "testChallenge5", Description = "descricao5", Points = 50, Type = "Weekly", MaxProgress = 4},
                };
                _ctx.Challenges.AddRange(challenges);
                _ctx.SaveChanges();

                // Add UserChallenges
                List<UserChallenges> userChallenges = challenges.Select(c => new UserChallenges {
                    Challenge = c,
                    User = users[2],
                    Progress = 0,
                    WasConcluded = false
                }).ToList();

                _ctx.UserChallenges.AddRange(userChallenges);
                _ctx.SaveChanges();

                return Ok();
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred");
            }
        }        
    }
}
#endif