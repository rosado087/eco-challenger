using EcoChallenger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly AppDbContext _ctx;

        public ProfileController(AppDbContext context)
        {
            _ctx = context;
        }

        /// <summary>
        /// Handles the action that gets the user information.
        /// Gets the information of the logged user.
        /// </summary>
        /// <param name="email">Is the email of the logged account</param>
        /// <returns>JSON result indicating success or failure. If returns failure also returns the error message. If success also returns the user information.</returns>

        [HttpGet("GetUserInfo/{email}")]
        public async Task<JsonResult> GetUserInfo(string email)  {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return new JsonResult(new {success = false, message = "O email é nulo"});

                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                    return new JsonResult(new {success = false, message = "O utilizador não existe"});

                var currentTag = await _ctx.TagUsers.Where(tg => tg.SelectedTag == true && tg.User.Id == user.Id).Select(tg => tg.Tag.Name).FirstOrDefaultAsync();

                return new JsonResult(new {success = true,  username = user.Username, email = user.Email, points = user.Points, tag = currentTag });
            }
            catch (Exception e)
            {
                return new JsonResult(new {success = false, message = e.Message });
            }      
        }


        /// <summary>
        /// Handles the action that gets all the tags which user owns
        /// Gets all the tags that the user owns.
        /// </summary>
        /// <param name="email">Is the email of the logged person</param>
        /// <returns>JSON result indicating success or failure. If returns failure also returns the error message. If returns success also returns the list of tags that the user owns.</returns>
        [HttpGet("GetTags/{email}")]
        public async Task<JsonResult> GetTags(string email)  {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return new JsonResult(new {success = false, message = "O email é nulo"});

                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                    return new JsonResult(new {success = false, message = "O utilizador não existe"});

                var tags = await _ctx.TagUsers.Where(tg => tg.User.Id == user.Id).Select(tg => tg.Tag.Name).ToListAsync();

                return new JsonResult(new {success = true,  list = tags });
            }
            catch (Exception e)
            {
                return new JsonResult(new {success = false, message = e.Message });
            }      
        }
    
        ///<summary>
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

        
[HttpGet("GetFriends")]
public async Task<IActionResult> GetFriends(string username)
{
    var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);
    if (user == null)
    {
        return NotFound(new { success = false, message = "Usuário não encontrado." });
    }

    var friends = await _ctx.Friendships
        .Where(f => f.UserId == user.Id)
        .Join(_ctx.Users, // Faz JOIN com a tabela Users
              f => f.FriendId, // Chave estrangeira na tabela Friendships
              u => u.Id, // ID do usuário na tabela Users
              (f, u) => u.Username) // Obtém apenas o nome do usuário amigo
        .ToListAsync();

    return Ok(new { success = true, friends });
}


    }
}