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


        [HttpPost("RegisterAccount")]
        public async Task<JsonResult> RegisterAccount([FromBody] User data)  {
            try
            {
                // Verifica se o email já existe
                var emailExists = await _ctx.Users.AnyAsync(x => x.Email == data.Email);

                if (emailExists)
                    return new JsonResult(new { success = false, message = "Este email já existe" });

                // Adiciona um novo usuário
                await _ctx.Users.AddAsync(data);
                await _ctx.SaveChangesAsync();

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