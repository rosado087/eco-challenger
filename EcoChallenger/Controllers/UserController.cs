using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }

        [HttpGet("GetGoogleId")]
        public async Task<JsonResult> GetGoogleId()
        {
            return new JsonResult(new { success = _configuration["GoogleClient:ClientId"] });
        }

        [HttpPost("AuthenticateGoogle")]
        public async Task<JsonResult> AuthenticateGoogle(string[] values)
        {
            if (_context.Users.Any()) {
                var user = await _context.Users.FirstAsync(u => u.GoogleToken == values[0] || u.Email == values[1]);

                if (user == null)
                {
                    return new JsonResult(new { success = false });
                }

                if (user.GoogleToken == null)
                {
                    user.GoogleToken = values[0];
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });

        }

        [HttpPut("SignUpGoogle")]
        public async Task<JsonResult> SignUpGoogle(string[] values)
        {

            var user = new User { Email = values[1], Username = values[0], GoogleToken = values[2] };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
        }

        [HttpPost("UserExists")]
        public async Task<JsonResult> UserExists(string username)
        {
            if(username == null) return new JsonResult(new { success = true });

            var user = await _context.Users.FirstAsync(u => u.Username == username);

            return new JsonResult((user != null) ? new {success = true} : new {success = false});
        }
    }
}
