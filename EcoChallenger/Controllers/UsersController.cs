using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<UsersController> _logger;
        private readonly IWebHostEnvironment _env;

        public UsersController(AppDbContext ctx, IWebHostEnvironment env, ILogger<UsersController> logger)
        {
            _ctx = ctx;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Returns list of information of all the users or that contain the searched word. 
        /// </summary>
        /// <param name="searchedName">The searched name or part of one </param>
        /// <returns>
        /// Returns list of users information.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult GetUsers([FromQuery]string? searchedName)
        {
            
            
            if(searchedName == null)
            {
                var users = _ctx.Users.Where(u => u.Id != UserContext.Id).Select(u => new { id = u.Id, username = u.Username, isBlocked = u.IsBlocked, isAdmin = u.IsAdmin}).ToList();
                return Ok(users);
            }
            else
            {
                var users = _ctx.Users.Where(u => u.Username.Contains(searchedName) && u.Id != UserContext.Id).Select(u => new { id = u.Id, username = u.Username, isBlocked = u.IsBlocked, isAdmin = u.IsAdmin }).ToList();
                return Ok(users);
            }

        }

        /// <summary>
        /// Changes the block state of the user. 
        /// </summary>
        /// <param name="id">Id of the chosen user </param>
        [Authorize(Roles = "Admin")]
        [HttpPost("block/{id}")]
        public async Task<JsonResult> ChangeBlock(int id)
        {
            _logger.LogInformation("BANG");
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
            _logger.LogInformation((user == null).ToString());
            if (user == null) return new JsonResult (new { success = false, message = "Este user não existe." });

            user.IsBlocked = !user.IsBlocked;

            _ctx.Update(user);
            await _ctx.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Estado mudado com successo." });
        }

        /// <summary>
        /// Changes the admin state of the user. 
        /// </summary>
        /// <param name="id">Id of the chosen user </param>
        /// <returns>
        /*[Authorize(Roles = "Admin")]
        [HttpPost("admin/{id}")]
        public async Task<JsonResult> ChangeAdmin(int id)
        {
            _logger.LogInformation("BOOM");
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
            _logger.LogInformation((user == null).ToString());
            if (user == null) return new JsonResult(new { success = false, message = "Este user não existe." });

            user.IsAdmin = !user.IsAdmin;

            _ctx.Update(user);
            await _ctx.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Estado mudado com successo." });
        }*/
    }
}
