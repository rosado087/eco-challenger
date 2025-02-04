using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public RegisterController(AppDbContext context){
            _ctx = context;
        }

        /// <summary>
        /// Handles the action that creates a new User.
        /// Verifies if the email which the User wrote is not being used already.
        /// If not, creates a new hashed password.
        /// Add the new User into the database.
        /// </summary>
        /// <param name="data">Contains the user's username, email address and password</param>
        /// <returns>JSON result indicating success or failure. If returns failure also returns the error message</returns>

        [HttpPost("RegisterAccount")]
        public async Task<JsonResult> RegisterAccount([FromBody] User data)  {
            try
            {
                // Verifica se o email já existe
                var emailExists = await _ctx.Users.AnyAsync(x => x.Email == data.Email);

                if (emailExists)
                    return new JsonResult(new { success = false, message = "Este email já existe" });


                //userTkn.User.Password = PasswordGenerator.GeneratePasswordHash(data.Password);

                data.Password = PasswordGenerator.GeneratePasswordHash(data.Password);

                // Adiciona um novo utilizador
                await _ctx.Users.AddAsync(data);
                await _ctx.SaveChangesAsync();
                // retorna uma resposta JSON com sucessor true
                return new JsonResult(new { success = true });
            }
            catch (Exception e)
            {
                // Retorna uma resposta JSON com a mensagem de erro
                return new JsonResult(new { success = false, message = e.Message });
            }                
        }
    }
}