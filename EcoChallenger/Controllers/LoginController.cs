using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext context, IConfiguration configuration)
        {
            _ctx = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<JsonResult> Login([FromBody] LoginRequestModel data)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(data.Email) || string.IsNullOrEmpty(data.Password))
                    return new JsonResult(new { success = false, message = "Email and password are required." });

                // Find the user by email
                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == data.Email);
                if (user == null)
                    return new JsonResult(new { success = false, message = "Invalid email or password." });

                // Verify the password
                bool isPasswordValid = PasswordGenerator.ComparePasswordWithHash(data.Password, user.Password);
                if (!isPasswordValid)
                    return new JsonResult(new { success = false, message = "Invalid email or password." });

                // Generate the token
                var userToken = TokenManager.CreateUserToken(user);

                // Save the token in the database
                _ctx.UserTokens.Add(userToken);
                await _ctx.SaveChangesAsync();

                // Return the token in the response
                return new JsonResult(new
                {
                    success = true,
                    message = "Login successful!",
                    token = userToken.Token,
                    Username = user.Username, // ✅ Return username
                    Email = user.Email // ✅ Return email
                });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error during login: {ex.Message}");

                // Return a generic error message
                return new JsonResult(new { success = false, message = "An error occurred during login." });
            }
        }

        [HttpGet("GetGoogleId")]
        public async Task<JsonResult> GetGoogleId()
        {
            return new JsonResult(new { success = _configuration["GoogleClient:ClientId"] });
        }

        [HttpPost("AuthenticateGoogle")]
        public async Task<JsonResult> AuthenticateGoogle(string[] values)
        {
            if (_ctx.Users.Any())
            {
                var user = await _ctx.Users.FirstAsync(u => u.GoogleToken == values[0] || u.Email == values[1]);

                if (user == null)
                {
                    return new JsonResult(new { success = false });
                }

                if (user.GoogleToken == null)
                {
                    user.GoogleToken = values[0];
                    _ctx.Users.Update(user);
                    await _ctx.SaveChangesAsync();
                }
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });

        }

        [HttpPut("SignUpGoogle")]
        public async Task<JsonResult> SignUpGoogle(string[] values)
        {

            var user = new User { Email = values[1], Username = values[0], GoogleToken = values[2] };
            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        [HttpPost("UserExists")]
        public async Task<JsonResult> UserExists(string username)
        {
            if (username == null) return new JsonResult(new { success = true });

            var user = await _ctx.Users.FirstAsync(u => u.Username == username);

            return new JsonResult((user != null) ? new { success = true } : new { success = false });
        }
    }
}
