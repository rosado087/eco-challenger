using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecoverPasswordController : ControllerBase
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
                "Para report a sua palavra passe, por favor aceda a este link: " + recoveryLink);

            return new JsonResult(new { success = true });
        }

        [HttpGet("CheckToken/{token}")]
        public JsonResult CheckToken(string token) {
            var userTkn = TokenManager.GetValidTokenRecord(_ctx, token);

            if(userTkn == null) return new JsonResult(new { success = false });

            return new JsonResult(new { success = true });
        }

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
