using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpPost("SendRecoveryEmail")]
        public async Task<JsonResult> SendRecoveryEmail([FromBody] SendRecoveryEmailModel data)
        {
            var users = await _ctx.Users.Where(u => u.Email == data.Email).ToListAsync();

            // When no users where found we want the frontend to think that
            // an email was sent, otherwise the person sending the email would
            // get confirmation that the account exists which could exploited
            // to do something else.            
            if(users.Count() == 0) return new JsonResult(new { success = true });

            // We want to generate a new recovery token and remove any old
            // ones the user might've had
            var userTokens = _ctx.UserTokens
                .Where(ut => ut.Type == UserToken.TokenType.RECOVERY && ut.User == users.First())
                .ToList();
            _ctx.UserTokens.RemoveRange(userTokens);
            
            var userToken = TokenManager.CreateUserToken(users.First());
            _ctx.UserTokens.Add(userToken);

            await _ctx.SaveChangesAsync();

            // Now we only need to send the email
            // To build the message we need to point the user to the correct URL
            string? baseUrl = _configuration.GetValue<string>("ApplicationSettings:FrontEndUrl");

            if(baseUrl == null){
                _logger.LogError("No application URL was configured in appsettings.json. Recovery emails are not being sent!");
                return new JsonResult(new {success = false});
            }

            // Make sure the baseURL value doesn't have a / at the end
            if(baseUrl.Substring(baseUrl.Length - 1) == "/")
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            string recoveryLink = baseUrl + "/reset-password/" + userToken.Token;
            await _emailService.SendEmailAsync(data.Email, "Echo-Challenger: Recuperação de Palavra-Passe", 
                "Para repore a sua palavra-passe, por favor aceda a este link: " + recoveryLink);

            return new JsonResult(new { success = true });
        }

        /// <summary>
        /// Handles the action that checks if a recovery token is valid or not
        /// </summary>
        /// <param name="token">The token to be checked, this comes as a route param</param>
        /// <returns>JSON result indicating success or failure</returns>
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
