using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class GamificationController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<GamificationController> _logger;
        

        public GamificationController(AppDbContext context, ILogger<GamificationController> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns challenges of the user.
        /// </summary>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a list of 
        /// challenges of the user</returns>
        [HttpGet("GetChallenges")]
        public async Task<JsonResult> GetChallenges()
        {
            try{
                var weeklyChallenges = await _ctx.UserChallenges.Where(x => x.User.Id == UserContext.Id && x.Challenge.Type == "Weekly").Select(x => new{challenge = x.Challenge, progress = x.Progress, wasConcluded = x.WasConcluded }).ToListAsync();
                var dailyChallenges = await _ctx.UserChallenges.Where(x => x.User.Id == UserContext.Id && x.Challenge.Type == "Daily").Select(x => new { challenge = x.Challenge, wasConcluded = x.WasConcluded }).ToListAsync();
                if (weeklyChallenges == null || dailyChallenges == null)
                    return new JsonResult(new { success = false, message = "Ocorreu um problema ao encontrar  os desafios atribuidos." });

                return new JsonResult(new
                {
                    success = true,
                    message = "Os desafios do utilizador foram encontrados com sucesso.",
                    dailyChallenges = dailyChallenges,
                    weeklyChallenges = weeklyChallenges,
                });
            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao buscar os desafios." });
            }

        }

        /// <summary>
        /// Completes a challenge.
        /// </summary>
        /// <param name="id">Id of the completed challenge.</param>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a message</returns>
        [HttpGet("CompleteChallenge/{id}")]
        public async Task<JsonResult> CompleteChallenge(int id)
        {
            try{
                var userChallenge = await _ctx.UserChallenges.Include(x => x.Challenge).Include(x => x.User).
                FirstOrDefaultAsync(x => x.Challenge.Id == id && x.User.Id == UserContext.Id);

                if (userChallenge == null)
                    return new JsonResult(new {success = false, message = "O desafio não existe." });

                if (userChallenge.WasConcluded == true || userChallenge.Progress == userChallenge.Challenge.MaxProgress)
                    return new JsonResult(new {success = false, message = "O desafio já está concluído." });

                userChallenge.User.Points += userChallenge.Challenge.Points;
                userChallenge.WasConcluded = true;
                userChallenge.Progress = userChallenge.Challenge.MaxProgress;
                await _ctx.SaveChangesAsync();

                return new JsonResult(new {
                    success = true, 
                    message = "Desafio concluido com sucesso.",
                });
            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao completar o desafio." });
            }
        }

        /// <summary>
        /// Adds progress on a challenge. If the progress reaches the end it completes the challenge.
        /// </summary>
        /// <param name="id">Id of the completed challenge.</param>
        /// <returns>JSON result indicating success or failure and a message.</returns>
        [HttpGet("AddProgress/{id}")]
        public async Task<JsonResult> AddProgress (int id)
        {
            try{
                var userChallenge = await _ctx.UserChallenges.Include(x => x.Challenge).Include(x => x.User).
                FirstOrDefaultAsync(x => x.Challenge.Id == id && x.User.Id == UserContext.Id);

                if (userChallenge == null)
                    return new JsonResult(new {success = false, message = "O desafio não existe." });

                if (userChallenge.WasConcluded == true)
                    return new JsonResult(new {success = false, message = "O desafio já está concluído." });

                userChallenge.Progress++;

                if (userChallenge.Progress == userChallenge.Challenge.MaxProgress){
                    userChallenge.User.Points += userChallenge.Challenge.Points;
                    userChallenge.WasConcluded = true;
                    await _ctx.SaveChangesAsync();

                    return new JsonResult(new { 
                        success = true, 
                        message = "Desafio concluido com sucesso.",
                    });
                }
                await _ctx.SaveChangesAsync();
                    
                return new JsonResult(new { 
                    success = true,
                    message = "Progresso adicionado com sucesso.",
                });
            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao adicionar progresso." });
            }
            
        }
    }
}
