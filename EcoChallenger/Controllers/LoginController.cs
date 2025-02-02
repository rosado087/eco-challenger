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
    public class LoginController : Controller
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
        /// Handles user authentication and JWT token generation.
        /// </summary>
        /// <param name="data">LoginModel containing email and password</param>
        /// <returns>JSON result indicating success, user data, and a JWT token</returns>
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Password))
            {
                _logger.LogWarning("Login attempt with missing email or password.");
                return BadRequest(new { success = false, message = "Email and password are required." });
            }

            // Find the user by email
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
            if (user == null)
            {
                _logger.LogWarning($"Login failed for email: {data.Email}. User not found.");
                return Unauthorized(new { success = false, message = "Invalid email or password." });
            }

            // Validate the password
            /*if (!PasswordGenerator.ValidatePassword(data.Password, user.Password))
            {
                _logger.LogWarning($"Login failed for email: {data.Email}. Incorrect password.");
                return Unauthorized(new { success = false, message = "Invalid email or password." });
            }*/

            // Directly compare the plain-text password
        if (data.Password != user.Password)
        {
            _logger.LogWarning($"Login failed for email: {data.Email}. Incorrect password.");
            return Unauthorized(new { success = false, message = "Invalid email or password." });
        }

            // Generate a JWT token
            string token = TokenManager.GenerateToken(user, _configuration);

            _logger.LogInformation($"User {data.Email} logged in successfully.");

            return Ok(new
{
    success = true,
    token,
    user = new
    {
        id = user.Id,
        email = user.Email,
        username = user.Username
    }
});

        

        }
    }
}
