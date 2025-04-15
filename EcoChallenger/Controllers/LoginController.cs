using EcoChallenger.Models;
using EcoChallenger.Services;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;

namespace EcoChallenger.Controllers
{
    public class LoginController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly DailyTaskService _dailyTaskService;
        private readonly WeeklyTaskService _weeklyTaskService;
        private Random _random;

        public LoginController(AppDbContext context, IConfiguration configuration, ILogger<LoginController> logger, IEnumerable<IHostedService> hostedServices)
        {
            _ctx = context;
            _configuration = configuration;
            _logger = logger;
            while(_dailyTaskService == null) _dailyTaskService = hostedServices.OfType<DailyTaskService>().FirstOrDefault();
            while(_weeklyTaskService == null) _weeklyTaskService = hostedServices.OfType<WeeklyTaskService>().FirstOrDefault();
            _random = new Random(int.Parse(DateTime.Now.ToString("yyyymmdd")));
        }

        /// <summary>
        /// Authenticates a user using email and password credentials.
        /// </summary>
        /// <param name="data">The login request data, including email and password.</param>
        /// <returns>
        /// A JSON result indicating success or failure. On success, returns a JWT token and basic user info.
        /// On failure, returns an appropriate error message.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<JsonResult> Login([FromBody] LoginRequestModel data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Email) || string.IsNullOrEmpty(data.Password))
                    return new JsonResult(new { success = false, message = "Email e palavra-passe são obrigatórios." });

                var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == data.Email);
                if (user == null)
                    return new JsonResult(new { success = false, message = "Email ou palavra-passe inválidos." });

                if(user.IsBlocked) return new JsonResult(new {success = false, message = "Esta conta foi bloqueada."});

                bool isPasswordValid = PasswordGenerator.ComparePasswordWithHash(data.Password, user.Password);
                if (!isPasswordValid)
                    return new JsonResult(new { success = false, message = "Email ou palavra-passe inválidos." });

                if (!string.IsNullOrEmpty(user.GoogleToken))
                    return new JsonResult(new { success = false, message = "Conta criada com GAuth, por favor use a opção de login correspondente." });

                string token = TokenManager.GenerateJWT(user);

                user.LastLogin = DateTime.UtcNow;
                _ctx.SaveChanges();

                return new JsonResult(new
                {
                    success = true,
                    token,
                    user = new {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        isAdmin = user.IsAdmin,
                        lastLogin = user.LastLogin
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao fazer login." });
            }
        }

        /// <summary>
        /// Retrieves the Google Client ID for Google Authentication.
        /// </summary>
        /// <returns>A JSON result containing the Google Client ID.</returns>
        [AllowAnonymous]
        [HttpGet("GetGoogleId")]
        public JsonResult GetGoogleId()
        {
            return new JsonResult(new { clientId = _configuration["GoogleClient:ClientId"] });
        }

        /// <summary>
        /// Authenticates a user using Google Authentication.
        /// If the user does not exist, a new account is created and assigned daily and weekly challenges.
        /// </summary>
        /// <param name="data">The Google authentication data, including email and token.</param>
        /// <returns>
        /// A JSON result indicating success or failure. On success, returns a JWT token and basic user info.
        /// On failure, returns an appropriate error message.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("AuthenticateGoogle")]
        public async Task<JsonResult> AuthenticateGoogle([FromBody] GAuthModel data)
        {
            try {

                var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
                string token;

                if (user == null)
                {
                    string username = data.Email.Split('@')[0];

                    User newUser = new User {
                        Username = username,
                        Email = data.Email,
                        GoogleToken = data.GoogleToken
                    };
                    await _ctx.Users.AddAsync(newUser);
                    await _ctx.SaveChangesAsync();

                    await _dailyTaskService.UpdateUserChallenges(newUser, false, _ctx);
                    await _weeklyTaskService.UpdateUserChallenges(newUser, false,_ctx);

                    await _ctx.SaveChangesAsync();
                    token = TokenManager.GenerateJWT(newUser);
                    
                    return new JsonResult(new { success = true, token, user = new {
                        id = newUser.Id,
                        username = newUser.Username,
                        email = newUser.Email,
                        isAdmin = newUser.IsAdmin,
                        lastLogin = newUser.LastLogin
                    }});
                }
                
                // If the user exists but has no token it means it was
                // created through the register form, we don't want to
                // authenticate him through here
                if(string.IsNullOrEmpty(user.GoogleToken)) {
                    return new JsonResult(new { success = false, message = "Esta conta não foi criada através do GAuth, por favor faça login pelo form."});
                }

                if (user.IsBlocked) return new JsonResult(new { success = false, message = "Esta conta foi bloqueada." });

                
                user.GoogleToken = data.GoogleToken; // Update user gtoken
                user.LastLogin = DateTime.UtcNow; // Update last login
                _ctx.Users.Update(user);
                await _ctx.SaveChangesAsync();

                token = TokenManager.GenerateJWT(user);

                return new JsonResult(new { success = true, token, user = new {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    isAdmin = user.IsAdmin
                }});
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao efetuar o login com Google Authentication."});
            }
        }
    }
}
