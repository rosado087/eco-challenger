using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class ChallengeController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<GamificationController> _logger;
        

        public ChallengeController(AppDbContext context, ILogger<GamificationController> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns challenges of the user.
        /// </summary>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a list of 
        /// challenges of the user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateChallenge")]
        public async Task<JsonResult> CreateChallenge([FromBody] ChallengeModel challengeModel)
        {
            try{
                var challenge = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Title == challengeModel.Title || c.Description == challengeModel.Description);

                if(challenge != null){
                    return new JsonResult(new{ success = false, message = "Já existe um desafio com esse titulo ou descrição"});
                }

                var newChallenge = new Challenge{
                    Title = challengeModel.Title,
                    Description = challengeModel.Description,
                    Points = challengeModel.Points,
                    Type = challengeModel.Type,
                    MaxProgress = challengeModel.MaxProgress
                };

                await _ctx.Challenges.AddAsync(newChallenge);
                await _ctx.SaveChangesAsync();

                var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == challengeModel.UserId);

                var userChallenge = new UserChallenges{
                    Challenge = newChallenge,
                    User = user,
                    Progress = 0,
                    WasConcluded = false
                };
                await _ctx.UserChallenges.AddAsync(userChallenge);
                await _ctx.SaveChangesAsync();
                
                return new JsonResult(new {success = true, message = "Desafio criado com sucesso"});

            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao adicionar o desafio." });
            }
            
        }

    }
}
