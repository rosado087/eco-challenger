using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoChallenger.Controllers
{
    public class RecoverPasswordController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecoverPasswordController> _logger;

        public RecoverPasswordController(AppDbContext context, IEmailService emailService, IConfiguration configuration, ILogger<RecoverPasswordController> logger) {
            _ctx = context;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Handles the action that sends a recovery email to set a new password.
        /// It first invalidates all the other recovery tokens the user had, and then
        /// creates the new one, assigns it to the user, and sends it to the email.
        /// </summary>
        /// <param name="data">Contains the user's email address</param>
        /// <returns>JSON result indicating success or failure</returns>
        [AllowAnonymous]
        [HttpPost("SendRecoveryEmail")]
        public async Task<JsonResult> SendRecoveryEmail([FromBody] SendRecoveryEmailModel data)
        {
            var user = _ctx.Users.Where(u => u.Email == data.Email).FirstOrDefault();

            // When no users where found we want the frontend to think that
            // an email was sent, otherwise the person sending the email would
            // get confirmation that the account exists which could exploited
            // to do something else.            
            if(user == null) return new JsonResult(new { success = true });

            // Same when checking if account was created with GAuth
            if(!string.IsNullOrEmpty(user.GoogleToken)) return new JsonResult(new { success = true });

            // We want to generate a new recovery token and remove any old
            // ones the user might've had
            var userTokens = _ctx.UserTokens
                .Where(ut => ut.Type == UserToken.TokenType.RECOVERY && ut.User == user)
                .ToList();
            _ctx.UserTokens.RemoveRange(userTokens);
            
            var userToken = TokenManager.CreateRecoveryUserToken(user);
            _ctx.UserTokens.Add(userToken);

            await _ctx.SaveChangesAsync();

            await _emailService.SendRecoveryEmailAsync(data.Email, userToken.Token);

            return new JsonResult(new { success = true });
        }

        /// <summary>
        /// Handles the action that checks if a recovery token is valid or not
        /// </summary>
        /// <param name="token">The token to be checked, this comes as a route param</param>
        /// <returns>JSON result indicating success or failure</returns>
        [AllowAnonymous]
        [HttpGet("CheckToken/{token}")]
        public JsonResult CheckToken(string token) {
            var userTkn = TokenManager.GetValidTokenRecord(_ctx, token);

            if(userTkn == null) return new JsonResult(new { success = false });

            return new JsonResult(new { success = true });
        }

        /// <summary>
        /// Handles the action that sets the new recovery password.
        /// This updates the User record with a new password in the hashed
        /// format and then removes the old UserToken record, making it invalid.
        /// </summary>
        /// <param name="data">Contains the user's email address</param>
        /// <returns>JSON result indicating success or failure</returns>
        [AllowAnonymous]
        [HttpPost("SetNewPassword")]
        public async Task<JsonResult> SetNewPassword([FromBody] SetNewPasswordModel data) {
            var userTkn = TokenManager.GetValidTokenRecord(_ctx, data.Token);
            if(userTkn == null) return new JsonResult(new { success = false, message = "A token de recuperação de palavra-passe é inválida." });
            
            userTkn.User.Password = PasswordGenerator.GeneratePasswordHash(data.Password);

            // Lets update the password and then remove the token record
            _ctx.Users.Update(userTkn.User);
            _ctx.UserTokens.Remove(userTkn);
            await _ctx.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }        
    }
}
