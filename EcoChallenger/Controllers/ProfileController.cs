using EcoChallenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V130.Animation;
using EcoChallenger.Controllers;
using EcoChallenger.Utils;

namespace EcoChallenger.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(AppDbContext context, ILogger<ProfileController> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        
        [HttpGet("GenerateToken")]
        public async Task<JsonResult> GenerateToken(){
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == UserContext.Id);
            if (user == null)
                return new JsonResult(new { success = false, message = "Ocorreu um problema ao encontrar o user"});

            string token = TokenManager.GenerateJWT(user);
            return new JsonResult(new { success = true, token, user = new {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                isAdmin = user.IsAdmin
            }});
        }

        /// <summary>
        /// Handles the action that gets the user information.
        /// Gets the information of the logged user.
        /// </summary>
        /// <param name="email">Email of the logged-in account</param>
        /// <returns>JSON result indicating success or failure.</returns>
        
        [HttpGet("GetUserInfo/{id}")]
        public async Task<JsonResult> GetUserInfo(int id)
        {
            try
            {
                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return new JsonResult(new { success = false, message = "O utilizador não existe" });

                var currentTag = await _ctx.TagUsers
                    .Where(tg => tg.SelectedTag && tg.User.Id == user.Id)
                    .Select(tg => tg.Tag.Name)
                    .FirstOrDefaultAsync();

                return new JsonResult(new { success = true, username = user.Username, email = user.Email, points = user.Points, tag = currentTag });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = e.Message });
            }
        }

        /// <summary>
        /// Handles the action that gets the user information.
        /// Gets the information of the logged user.
        /// </summary>
        /// <param name="email">Email of the logged-in account</param>
        /// <returns>JSON result indicating success or failure.</returns>
        
        [HttpPost("EditUserInfo")]
        public async Task<JsonResult> EditUserInfo([FromBody] ProfileEditModel profile)
        {
            try
            {
                profile.Validate();
                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == profile.Id);

                if (user == null)
                    return new JsonResult(new { success = false, message = "O utilizador não existe" });
                
                user.Username = profile.Username;

                var currentTag = await _ctx.TagUsers
                    .Where(tg => tg.SelectedTag && tg.User.Id == user.Id)
                    .FirstOrDefaultAsync();
                
                if (currentTag != null)
                    currentTag.SelectedTag = false;

                var newCurrentTag = await _ctx.TagUsers.Where(tg => tg.Tag.Name == profile.Tag && tg.User.Id == user.Id).FirstOrDefaultAsync();
                
                if (newCurrentTag != null)
                    newCurrentTag.SelectedTag = true;

                await _ctx.SaveChangesAsync();

                var currentTagName = await _ctx.TagUsers
                    .Where(tg => tg.SelectedTag && tg.User.Id == user.Id)
                    .Select(tg => tg.Tag.Name)
                    .FirstOrDefaultAsync();

                await GenerateToken();

                return new JsonResult(new { success = true, username = user.Username, email = user.Email, points = user.Points, tag = currentTagName });
            }
            catch (ArgumentException e){
                return new JsonResult(new { success = false, message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao atualizar os seus dados" });
            }
        }

        /// <summary>
        /// Gets all tags that the user owns.
        /// </summary>
        /// <param name="email">Email of the logged-in user</param>
        /// <returns>JSON result containing a list of tags.</returns>
        
        [HttpGet("GetTags/{id}")]
        public async Task<JsonResult> GetTags(int id)
        {
            try
            {
                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return new JsonResult(new { success = false, message = "O utilizador não existe" });

                var tags = await _ctx.TagUsers
                    .Where(tg => tg.User.Id == user.Id)
                    .Select(tg => tg.Tag.Name)
                    .ToListAsync();

                return new JsonResult(new { success = true, list = tags });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Não foi possível encontrar as suas tags" });
            }
        }

        /// <summary>
        /// Retrieves a list of users containing a searched username.
        /// </summary>
        /// <param name="values">Array containing the username of the requesting user and the search term.</param>
        /// <returns>JSON result with a list of usernames.</returns>
        
        [HttpPost("UserList")]
        public async Task<JsonResult> UserList(string[] values)
        {
            if (values.Length < 2)
                return new JsonResult(new { success = false, message = "Parâmetros inválidos" });

            var currentUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == values[0]);

            if (currentUser == null)
                return new JsonResult(new { success = false, message = "Usuário não encontrado" });

            var friends = await _ctx.Friendships
                .Where(f => f.UserId == currentUser.Id)
                .Select(f => f.FriendId)
                .ToListAsync();

            var users = await _ctx.Users
                .Where(u => u.Username.Contains(values[1]) && !u.Username.Equals(values[0]) && !friends.Contains(u.Id))
                .Select(u => u.Username)
                .ToListAsync();

            return new JsonResult(new { success = true, usernames = users });
        }

        /// <summary>
        /// Adds a user as a friend.
        /// </summary>
        /// <param name="values">Array containing the username of the requester and the friend’s username.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost("AddFriend")]
        public async Task<JsonResult> AddFriend(string[] values)
        {
            if (values.Length < 2)
                return new JsonResult(new { success = false, message = "Parâmetros inválidos" });

            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == values[0]);
            var friend = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == values[1]);

            if (user == null || friend == null)
                return new JsonResult(new { success = false, message = "Usuário ou amigo não encontrado" });

            var friendshipExists = await _ctx.Friendships
                .AnyAsync(f => f.UserId == user.Id && f.FriendId == friend.Id);

            if (friendshipExists)
                return new JsonResult(new { success = false, message = "Usuário já é amigo" });

            _ctx.Friendships.Add(new Friend { UserId = user.Id, FriendId = friend.Id });
            await _ctx.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        /// <summary>
        /// Retrieves a list of a user's friends.
        /// </summary>
        /// <param name="id">Id of the requested user.</param>
        /// <returns>JSON result with the list of friends.</returns>
        
        [HttpGet("GetFriends/{id}")]
        public async Task<IActionResult> GetFriends(int id )
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return new JsonResult(new { success = false, message = "Utilizador não encontrado." });

            var friendList = await _ctx.Friendships
                .Where(f => f.UserId == user.Id)
                .Join(_ctx.Users, 
                    f => f.FriendId, 
                    u => u.Id, 
                    (f, u) => new { u.Username, u.Email })
                .ToListAsync();

            return new JsonResult(new { success = true, friends = friendList });
        }


    }
}
