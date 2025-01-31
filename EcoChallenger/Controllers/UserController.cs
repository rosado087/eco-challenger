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

        [HttpGet("id")]
        public string GetId()
        {
            Console.WriteLine(_configuration["Google:ClientId"]);
            return _configuration["Google:ClientId"];
        } 

        [HttpPost("authenticate")]
        public async Task<JsonResult> AuthenticateGoogle(string token, string email)
        {

            var user = await _context.Users.FirstAsync(t => t.GoogleToken == token);

            
            if (user == null)
            {
                user = await _context.Users.FirstAsync(u => u.Email == email);
                
                if(user != null)
                {
                    user.GoogleToken = token;
                    _context.Users.Update(user);
                    return new JsonResult(new { success = true });
                }

                return new JsonResult(new { success = false });
            }

            return new JsonResult(new { success = true });

        }

        [HttpPut("signup-google")]
        public async Task<JsonResult> signUpGoogle(string username, string email, string token)
        {

            var user = new User { Email = email, Username = username, GoogleToken = token };
            _context.Users.Add(user);

                return new JsonResult(new { success = true });
        }

        [HttpPost("user-exists")]
        public async Task<JsonResult> userExists(string username)
        {
            var user = await _context.Users.FirstAsync(u => u.Username == username);

            return new JsonResult((user != null) ? new {success = true} : new {success = false});
        }
    }
}
