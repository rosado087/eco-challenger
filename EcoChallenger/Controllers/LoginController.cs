using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EcoChallenger.Models;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext context, IConfiguration configuration, ILogger<LoginController> logger)
        {
            _ctx = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handles the login process by validating user credentials.
        /// </summary>
        /// <param name="data">LoginModel containing email and password</param>
        /// <returns>JSON result indicating success or failure, along with a JWT if successful</returns>
        [HttpPost("Authenticate")]
        public async Task<JsonResult> Authenticate([FromBody] LoginModel data)
        {
            // Find the user by email
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "Invalid email or password." });
            }

            // Validate the password
            if (!PasswordGenerator.ValidatePassword(data.Password, user.Password))
            {
                return new JsonResult(new { success = false, message = "Invalid email or password." });
            }

            // Generate a JWT token
            string token = TokenManager.GenerateToken(user, _configuration);
            return new JsonResult(new { success = true, token });
        }
    }
}
