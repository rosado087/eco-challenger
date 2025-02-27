using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ProfileController(AppDbContext context){
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
    }
}