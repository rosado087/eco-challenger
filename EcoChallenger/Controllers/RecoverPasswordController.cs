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

        public RecoverPasswordController(AppDbContext context, IEmailService emailService) {
            _ctx = context;
            _emailService = emailService;
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
            
            var userToken = CreateUserToken(users.First());
            _ctx.UserTokens.Add(userToken);

            await _ctx.SaveChangesAsync();

            // Now we only need to send the email
            // To build the message we need to point the user to the correct
            // URL so we fetch it from the request headers and replace some parts
            string recoveryLink = Request.Headers["Referer"].ToString() + "/reset-password/" + userToken.Token;
            await _emailService.SendEmailAsync(data.Email, "Echo-Challenger: Recuperação de Palavra-Passe", 
                "Para report a sua palavra passe, por favor aceda a este link: " + recoveryLink);

            return new JsonResult(new { success = true });
        }

        private UserToken CreateUserToken(User user) {
            string newToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return new UserToken {
                User = user,
                Token = newToken,
                Type = UserToken.TokenType.RECOVERY,
                CreationDate = DateTime.Now,
                Duration = TimeSpan.FromHours(4) // Token lasts 4 hours
            };
        }
    }
}
