using EcoChallenger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ProfileController(AppDbContext context)
        {
            _ctx = context;
        }

        /// <summary>
        /// Handles the action getting a list of users that contain a searched name.
        /// </summary>
        /// <param name="username"> is a part of the username of user/s o</param>
        /// <returns>JSON result with list of usernames. If returns failure also returns the error message</returns>

        [HttpPost("UserList")]
        public async Task<JsonResult> UserList(string[] values) {

            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == values[0]);
            var friends = await _ctx.Friendships.Where(u => u.UserId == user.Id).Select(u => u.FriendId).ToListAsync();
            var users = await _ctx.Users.Where( u => u.Username.Contains(values[1]) && !u.Username.Equals(values[0]) && !friends.Contains(u.Id))
                              .Select(u => u.Username).ToListAsync();
            
            
            
            return new JsonResult(new { usernames = users });
        }

        /// <summary>
        /// Handles the action that adds a User as a friend.
        /// Verifies if User is already in friend list.
        /// Updates the friend list in the User into the database.
        /// </summary>
        /// <param name="data">Contains the user's username, email address and password</param>
        /// <returns>JSON result indicating success or failure. If returns failure also returns the error message</returns>
        [HttpPost("AddFriend")]
        public async Task<JsonResult> AddFriend(string[] values)
        {
            var user = await _ctx.Users.FirstAsync(u => u.Username == values[0]);
            var friend = await _ctx.Users.FirstAsync(u => u.Username == values[1]);
            
            var friendship = new Friend { UserId = user.Id, FriendId = friend.Id };
            _ctx.Friendships.Add(new Friend { UserId = user.Id, FriendId = friend.Id });
            await _ctx.SaveChangesAsync();

            return new JsonResult(new { result = true });
        }
    }
}
